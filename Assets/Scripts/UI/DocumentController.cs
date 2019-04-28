using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DocumentController : MonoBehaviour
{
    [SerializeField] private Text _title;
    [SerializeField] private Text _content;

    public void Initialize(Document document)
    {
        _title.text = document.Title;
        _content.text = document.Content;
    }
}
