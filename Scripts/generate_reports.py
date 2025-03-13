import os
from collections import defaultdict
import re  

def count_test_generated_logs(file_path):
    if not os.path.exists(file_path):
        print("Le fichier game_logs.txt n'existe pas !")
        return
    
    Test_cases_count = 0
    Test_Failed_count = 0
    Test_Passed_count = 0
    oracle_data = defaultdict(lambda: {'passed': 0, 'failed': 0, 'count': 0, 'logs_passed': [], 'logs_failed': []})

    with open(file_path, 'r', encoding='utf-8') as file:
        for line in file:
            if "TestGenerated" in line:
                Test_cases_count += 1
            if "TestFailed" in line:
                Test_Failed_count += 1
            if "TestPassed" in line:
                Test_Passed_count += 1
            if "ORACLE" in line:
                match = re.search(r"ORACLE\s+(\S+)", line)  # Extraction du nom de l'ORACLE
                if match:
                    oracle_name = match.group(1)
                    oracle_data[oracle_name]['count'] += 1  
                    if "TestFailed" in line:
                        oracle_data[oracle_name]['failed'] += 1  
                        oracle_data[oracle_name]['logs_failed'].append(line.strip())  # Stocker le log complet
                    if "TestPassed" in line:
                        oracle_data[oracle_name]['passed'] += 1  
                        oracle_data[oracle_name]['logs_passed'].append(line.strip())  # Stocker le log complet

    # Création du fichier HTML avec f-string
    html_report = f"""
    <html>
    <head>
        <title>Unity Log Report</title>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 20px; }}
            h1 {{ color: #333; }}
            table {{ border-collapse: collapse; width: 100%; }}
            table, th, td {{ border: 1px solid black; }}
            th, td {{ padding: 10px; text-align: center; }}
            th {{ background-color: #f2f2f2; }}
            .section {{ margin-top: 30px; }}
            .log-box {{ background: #f8f8f8; padding: 10px; border-radius: 5px; margin: 5px 0; }}
            .log-passed {{ color: green; }}
            .log-failed {{ color: red; }}
        </style>
    </head>
    <body>
        <h1>Unity Log Analysis Report</h1>
        <h2>Summary</h2>
        <p><strong>Nombre de logs contenant 'TestGenerated':</strong> {Test_cases_count}</p>
        <p><strong>Nombre de logs contenant 'TestFailed':</strong> {Test_Failed_count}</p>
        <p><strong>Nombre de logs contenant 'TestPassed':</strong> {Test_Passed_count}</p>
    """

    # Tableau récapitulatif des ORACLE
    html_report += """
        <div class="section">
            <h2>ORACLE Test Results</h2>
            <table>
                <tr>
                    <th>ORACLE</th>
                    <th>Instances</th>
                    <th>Test Passed</th>
                    <th>Test Failed</th>
                </tr>
    """

    for oracle, data in oracle_data.items():
        html_report += f"""
                <tr>
                    <td>{oracle}</td>
                    <td>{data['count']}</td>
                    <td>{data['passed']}</td>
                    <td>{data['failed']}</td>
                </tr>
        """

    html_report += """
            </table>
        </div>
    """

    # Affichage détaillé des logs par ORACLE
    for oracle, data in oracle_data.items():
        html_report += f"""
        <div class="section">
            <h2>Logs pour ORACLE: {oracle}</h2>
        """
        
        if data['logs_passed']:
            html_report += "<h3 style='color: green;'>Tests Réussis</h3>"
            for log in data['logs_passed']:
                html_report += f"<div class='log-box log-passed'>{log}</div>"
        
        if data['logs_failed']:
            html_report += "<h3 style='color: red;'>Tests Échoués</h3>"
            for log in data['logs_failed']:
                html_report += f"<div class='log-box log-failed'>{log}</div>"

        html_report += "</div>"

    html_report += """
    </body>
    </html>
    """

    with open("report_HTML.html", "w", encoding="utf-8") as report_file:
        report_file.write(html_report)

    print("Le rapport HTML a été généré avec succès : report_HTML.html")

count_test_generated_logs("Assets\Scripts\game_logs.txt")