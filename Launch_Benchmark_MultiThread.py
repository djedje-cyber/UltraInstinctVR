import subprocess
import time
import os
from multiprocessing import Pool
import csv
import psutil
import uuid
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
REPEAT_COUNT = 1
TIMEOUT = 500  # 5 minutes


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
def wait_for_action_done(log_file_path, timeout=TIMEOUT, process=None):
    start_time = time.time()

    while time.time() - start_time < timeout:
        if os.path.exists(log_file_path):
            try:
                with open(log_file_path, "r", encoding="utf-8") as f:
                    if "ACTION_DONE" in f.read():
                        print("✅ ACTION_DONE détecté.")
                        time.sleep(100)
                        if process:
                            process.terminate()
                        return True
            except Exception:
                pass

        time.sleep(1)

    print("⚠️ Timeout atteint sans ACTION_DONE.")
    if process:
        process.terminate()
    return False


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

    success = wait_for_action_done(
        log_file_path=log_file_path,
        process=process
    )

    # CPU fin (avant disparition)
    end_cpu = get_total_cpu_time(ps_proc)

    process.wait()

    cpu_used = max(0.0, end_cpu - start_cpu)

    # ⏱️ end time réel
    end_dt = datetime.utcnow().isoformat()

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
# Main
# =========================
if __name__ == "__main__":
    run_projects_in_parallel()