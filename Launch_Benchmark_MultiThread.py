import subprocess
import time
import os
from multiprocessing import Pool
import csv
import psutil
import uuid
import shutil
from datetime import datetime



# Chemins vers les éditeurs Unity
UNITY_PATHS = [
    r"C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.62f1\\Editor\\Unity.exe"
]

# Chemins vers les projets Unity
PROJECT_PATHS = [
    r"C:\\Users\\glongfil\\\Documents\GitHub\\\UltraInstinctVR\\CodeBase"
]


BATCH_SIZE = 1 
REPEAT_COUNT = 2
TIMEOUT = 300  # 5 minutes


# =========================
# Utils CPU
# =========================
def get_total_cpu_time(proc: psutil.Process):
    """CPU total du process + enfants."""
    total = 0.0

    try:
        cpu = proc.cpu_times()
        total += cpu.user + cpu.system
    except Exception:
        pass

    try:
        for child in proc.children(recursive=True):
            try:
                c = child.cpu_times()
                total += c.user + c.system
            except Exception:
                pass
    except Exception:
        pass

    return total


# =========================
# CSV Path helper
# =========================
def create_csv_for_project(project_path):
    cpu_dir = os.path.join(project_path, "Logs", "cpu_time")
    os.makedirs(cpu_dir, exist_ok=True)

    short_uuid = str(uuid.uuid4())[:6]
    csv_path = os.path.join(cpu_dir, f"cpu_time_{short_uuid}.csv")

    return csv_path


# =========================
# CSV Writer
# =========================
def write_csv_row(csv_path, project_index, iteration, success, cpu_time, start_time, end_time):
    file_exists = os.path.exists(csv_path)

    with open(csv_path, "a", newline="", encoding="utf-8") as f:
        writer = csv.writer(f)

        if not file_exists:
            writer.writerow([
                "project",
                "iteration",
                "success",
                "cpu_time_sec",
                "start_time",
                "end_time"
            ])

        writer.writerow([
            project_index + 1,
            iteration + 1,
            success,
            round(cpu_time, 3),
            start_time,
            end_time
        ])


# =========================
# Wait for ACTION_DONE
# =========================
def wait_for_action_done(log_file_path, timeout=TIMEOUT, process=None, ps_proc=None):
    start_time = time.time()
    last_cpu = 0.0

    while time.time() - start_time < timeout:
        if ps_proc:
            try:
                last_cpu = get_total_cpu_time(ps_proc)
            except Exception:
                pass

        if os.path.exists(log_file_path):
            try:
                with open(log_file_path, "r", encoding="utf-8") as f:
                    if "ACTION_DONE" in f.read():
                        print("✅ ACTION_DONE détecté.")
                        end_dt = datetime.utcnow().isoformat()
                        if ps_proc:
                            try:
                                last_cpu = get_total_cpu_time(ps_proc)
                            except Exception:
                                pass
                        time.sleep(100)
                        if process:
                            process.terminate()
                        return True, last_cpu, end_dt
            except Exception:
                pass

        time.sleep(1)

    print("⚠️ Timeout atteint sans ACTION_DONE.")
    end_dt = datetime.utcnow().isoformat()
    if ps_proc:
        try:
            last_cpu = get_total_cpu_time(ps_proc)
        except Exception:
            pass
    if process:
        process.terminate()
    return False, last_cpu, end_dt



# =========================
# Run Unity once
# =========================
def run_unity_once(project_path, unity_path, index, iteration):
    print(f"\n🚀 Lancement Unity - Projet {index+1} | Itération {iteration+1}")

    # ⏱️ start time réel
    start_dt = datetime.utcnow().isoformat()

    log_file_path = os.path.join(project_path, "Assets", "Scripts", "game_logs.txt")

    if os.path.exists(log_file_path):
        os.remove(log_file_path)

    command = [
        unity_path,
        "-projectPath", project_path,
        "-runTests",
        "-testPlatform", "Playmode",
        "-enableCodeCoverage"
    ]

    process = subprocess.Popen(
        command,
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL
    )

    ps_proc = psutil.Process(process.pid)

    # CPU début
    start_cpu = get_total_cpu_time(ps_proc)

    success, end_cpu, end_dt = wait_for_action_done(
        log_file_path=log_file_path,
        process=process,
        ps_proc=ps_proc
    )

    process.wait()

    # Copie du coverage, que le run ait réussi ou non
    copy_coverage_summary(project_path, run_number=iteration + 1)
    copy_full_coverage_folder(project_path, run_number=iteration + 1)

    cpu_used = max(0.0, end_cpu - start_cpu)

    return (index, iteration, success, cpu_used, start_dt, end_dt)



# =========================
# Iterations par projet
# =========================
def run_project_iterations(project_index):
    project_path = PROJECT_PATHS[project_index]
    unity_path = UNITY_PATHS[project_index]

    csv_path = create_csv_for_project(project_path)

    results = []

    for i in range(REPEAT_COUNT):
        try:
            index, iteration, success, cpu_used, start_dt, end_dt = run_unity_once(
                project_path,
                unity_path,
                project_index,
                i
            )

            write_csv_row(
                csv_path,
                index,
                iteration,
                success,
                cpu_used,
                start_dt,
                end_dt
            )

            if success:
                print(f"✔️ Projet {index+1} | Itération {iteration+1} réussie.")
            else:
                print(f"❌ Projet {index+1} | Itération {iteration+1} échouée.")

            print(f"🧠 CPU utilisé: {cpu_used:.3f} sec")

            results.append(success)

        except Exception as e:
            print(f"💥 Erreur durant l'itération {i+1} du projet {project_index+1} : {e}")
            results.append(False)

        time.sleep(3)

    return results


# =========================
# Parallel runner
# =========================
def run_projects_in_parallel():
    if len(PROJECT_PATHS) != len(UNITY_PATHS):
        print("⚠️ Le nombre de projets et d'éditeurs ne correspond pas.")
        return

    with Pool(processes=min(BATCH_SIZE, len(PROJECT_PATHS))) as pool:
        all_results = pool.map(run_project_iterations, range(len(PROJECT_PATHS)))

    for idx, results in enumerate(all_results):
        success_count = results.count(True)
        fail_count = results.count(False)
        print(f"\n📊 Résumé du projet {idx+1} : {success_count} succès / {REPEAT_COUNT} | {fail_count} échecs")


# =========================
# Copie du rapport de couverture
# =========================
def copy_coverage_summary(project_path, run_number):
    src_path = os.path.join(project_path, "CodeCoverage", "Report","Summary.json")

    if not os.path.exists(src_path):
        print(f"⚠️ Summary.json introuvable à {src_path}")
        return None

    dest_dir = os.path.join(project_path, "Logs", "coverage_raw")
    os.makedirs(dest_dir, exist_ok=True)

    short_uuid = str(uuid.uuid4())[:6]
    date_str = datetime.now().strftime("%Y%m%d")

    dest_name = f"raw_coverage_log_{short_uuid}_{date_str}_Run_{run_number}.json"
    dest_path = os.path.join(dest_dir, dest_name)

    shutil.copy2(src_path, dest_path)
    print(f"📄 Coverage summary copié vers {dest_path}")

    return dest_path


# =========================
# Copie complète du dossier CodeCoverage
# =========================
def copy_full_coverage_folder(project_path, run_number):
    src_dir = os.path.join(project_path, "CodeCoverage")

    if not os.path.exists(src_dir):
        print(f"⚠️ Dossier CodeCoverage introuvable à {src_dir}")
        return None

    all_coverage_dir = os.path.join(project_path, "Logs", "ALL_Coverage")
    os.makedirs(all_coverage_dir, exist_ok=True)

    dest_dir = os.path.join(all_coverage_dir, f"CodeCoverage_RUN_{run_number}")

    if os.path.exists(dest_dir):
        shutil.rmtree(dest_dir)

    shutil.copytree(src_dir, dest_dir)
    print(f"📁 Dossier CodeCoverage copié vers {dest_dir}")

    return dest_dir




# =========================
# Main
# =========================
if __name__ == "__main__":
    run_projects_in_parallel()