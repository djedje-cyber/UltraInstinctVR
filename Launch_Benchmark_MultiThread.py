import subprocess
import time
import os
from multiprocessing import Pool

# Exemple de chemin d'éditeur et de projet pour chaque projet
UNITY_PATHS = [
    r"C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.37f1\\Editor\\Unity.exe",
    r"C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.37f1\\Editor\\Unity.exe",
    r"C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.37f1\\Editor\\Unity.exe",
    r"C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.37f1\\Editor\\Unity.exe",
    r"C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.37f1\\Editor\\Unity.exe",
    # Ajoute ici d'autres chemins vers les éditeurs Unity pour chaque projet
]

PROJECT_PATHS = [
    r"C:\\Users\\glongfil\\EscapeRoom",
    r"C:\\Users\\glongfil\\EscapeRoom",
    r"C:\\Users\\glongfil\\EscapeRoom",
    r"C:\\Users\\glongfil\\EscapeRoom",
    r"C:\\Users\\glongfil\\EscapeRoom"


    # Ajoute ici d'autres chemins pour les autres projets
]

LOG_FILE_PATH = "game_logs.txt"  # Tu peux ajuster ça selon la structure de tes logs
REPEAT_COUNT = 30
TIMEOUT = 1200  # 20 minutes = 1200 secondes


def wait_for_action_done(timeout=TIMEOUT, process=None):
    start_time = time.time()
    while time.time() - start_time < timeout:
        if os.path.exists(LOG_FILE_PATH):
            with open(LOG_FILE_PATH, "r", encoding="utf-8") as f:
                if "ACTION_DONE" in f.read():
                    print("✅ ACTION_DONE détecté.")
                    if process:
                        process.terminate()  # Ferme Unity après la détection de "ACTION_DONE"
                    return True
        time.sleep(1)
    print("⚠️ Timeout atteint sans ACTION_DONE.")
    if process:
        process.terminate()  # Ferme Unity si on atteint le timeout
    return False


def run_unity_once(project_path, unity_path, index):
    print(f"\n🚀 Lancement Unity - projet {index+1}")

    log_file_path = os.path.join(project_path, "Assets", "Scripts", "game_logs.txt")

    # Nettoyer les anciens logs
    if os.path.exists(log_file_path):
        os.remove(log_file_path)

    command = [
        unity_path,
        "-projectPath", project_path
    ]

    process = subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    success = wait_for_action_done(process=process)
    process.wait()

    return success


def run_projects_in_parallel():
    # Vérifier si le nombre de projets et d'éditeurs est cohérent
    if len(PROJECT_PATHS) > len(UNITY_PATHS):
        print("⚠️ Il y a plus de projets que d'éditeurs Unity disponibles. Les éditeurs seront utilisés de manière cyclique.")
    
    # S'assurer que REPEAT_COUNT ne dépasse pas la taille des projets disponibles
    repeat_count = min(REPEAT_COUNT, len(PROJECT_PATHS))

    # S'assurer que les arguments pour chaque projet sont bien définis
    args = [(PROJECT_PATHS[i % len(PROJECT_PATHS)], UNITY_PATHS[i % len(UNITY_PATHS)], i) for i in range(repeat_count)]
    
    with Pool(5) as pool:
        # Lancer les projets en parallèle
        results = pool.starmap(run_unity_once, args)

        # Vérifier les résultats de chaque processus
        for i, result in enumerate(results):
            if result:
                print(f"✔️ Itération {i+1} terminée avec succès.")
            else:
                print(f"❌ Échec à l'itération {i+1}, arrêt du script.")
                break


if __name__ == "__main__":
    run_projects_in_parallel()