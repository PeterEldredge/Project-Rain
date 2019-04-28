using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Events
{
    public class UIController : GameEventUserObject
    {
        [SerializeField] private GameObject _screenTint;
        [SerializeField] private KeypadController _keypad;
        [SerializeField] private TerminalController _terminal;
		[SerializeField] private DocumentController _document;

        public UnityEvent onUIOpened = new UnityEvent();
        public void OnUIOpened()
        {
			_screenTint.SetActive(true);

            onUIOpened.Invoke();
        }

        public UnityEvent onUIClosed = new UnityEvent();
        public void OnUIClosed()
        {
			_screenTint.SetActive(false);

            onUIClosed.Invoke();
        }

        public override void Subscribe()
        {
            EventManager.AddListener<KeypadUsedEvent>(KeypadUsed);
            EventManager.AddListener<TerminalUsedEvent>(TerminalUsed);

            EventManager.AddListener<DocumentCollectedEvent>(DocumentCollected);
			EventManager.AddListener<ItemCollectedEvent>(ItemCollected);
			EventManager.AddListener<JournalCollectedEvent>(JournalCollected);
        }

        public override void Unsubscribe()
        {
            EventManager.RemoveListener<KeypadUsedEvent>(KeypadUsed);
            EventManager.RemoveListener<TerminalUsedEvent>(TerminalUsed);

			EventManager.RemoveListener<DocumentCollectedEvent>(DocumentCollected);
			EventManager.RemoveListener<ItemCollectedEvent>(ItemCollected);
			EventManager.RemoveListener<JournalCollectedEvent>(JournalCollected);
        }


		//Needs to be refactored, should use IUIItem interface
		//Shouldn't need to manually type all this
        public void KeypadUsed(KeypadUsedEvent eventArgs)
        {
            _keypad.gameObject.SetActive(true);
            _keypad.Initialize(eventArgs.Code);

            OnUIOpened();
        }

        public void KeypadClosed()
        {
            _keypad.gameObject.SetActive(false);

            OnUIClosed();
        }

        public void TerminalUsed(TerminalUsedEvent eventArgs)
        {
            _terminal.gameObject.SetActive(true);
            _terminal.Initialize(eventArgs.TerminalContent);

            OnUIOpened();
        }

        public void TerminalClosed()
        {
            _terminal.gameObject.SetActive(false);

            OnUIClosed();
        }

        public void DocumentCollected(DocumentCollectedEvent eventArgs)
        {
            _document.gameObject.SetActive(true);
			_document.Initialize(eventArgs.Document);

			OnUIOpened();
        }

		public void DocumentClosed()
		{
            _document.gameObject.SetActive(false);

			OnUIClosed();
		}

		public void ItemCollected(ItemCollectedEvent eventArgs)
		{
			
		}

		public void ItemClosed()
		{

		}

		public void JournalCollected(JournalCollectedEvent eventArgs)
		{
			
		}

		public void JournalClosed()
		{
			
		}
    }
}
