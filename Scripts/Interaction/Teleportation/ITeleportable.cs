using System.Collections;
using UnityEngine;
using System;

[Serializable] // Permet de l'afficher dans l'Inspector
public abstract class ITeleportable : MonoBehaviour
{
    public abstract IEnumerator Execute(); // Méthode pour exécuter la téléportation
    public Action OnTeleportComplete; // Événement pour signaler la fin
}
