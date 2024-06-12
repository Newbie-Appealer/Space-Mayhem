using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class JournalSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _journalDATE;
    [SerializeField] private TextMeshProUGUI _journalTEXT;

    public void F_InitSlot(int v_date, string v_text)
    {
        _journalDATE.text = v_date + " day";
        _journalTEXT.text = v_text;
    }
}
 