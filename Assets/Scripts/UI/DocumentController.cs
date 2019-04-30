using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DocumentController : MonoBehaviour
{
    [SerializeField] private Text _title;
    [SerializeField] private Text _content;

    //May need to trigger this in the spot where the document actually is picked up
    public UnityEvent onDocumentPickedUp = new UnityEvent();
	private void OnDocumentPickedUp()
	{
		onDocumentPickedUp.Invoke();
	}

	public UnityEvent onDocumentClosed = new UnityEvent();
	private void OnDocumentClosed()
	{
		onDocumentClosed.Invoke();
	}

    private LayoutElement _contentLayoutElement;

    private void Awake()
    {
        _contentLayoutElement = _content.gameObject.GetComponent<LayoutElement>();
    }

    public void Initialize(Document document)
    {
        OnDocumentPickedUp();

        _title.text = document.Title;
        _content.text = document.Content;
        _contentLayoutElement.preferredHeight = _content.preferredHeight;
    }
}
