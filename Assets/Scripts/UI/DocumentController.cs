using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DocumentController : MonoBehaviour
{
    [SerializeField] private Text _title;
    [SerializeField] private Text _content;

    private LayoutElement _contentLayoutElement;

    private void Awake()
    {
        _contentLayoutElement = _content.gameObject.GetComponent<LayoutElement>();
    }

    public void Initialize(Document document)
    {
        _title.text = document.Title;
        _content.text = document.Content;
        _contentLayoutElement.preferredHeight = _content.preferredHeight;
    }
}
