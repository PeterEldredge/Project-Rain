using UnityEngine;

namespace PixelCrushers.DialogueSystem.OpsiveUCCSupport
{

    /// <summary>
    /// This is a Dialogue System Trigger that serves as a UCC Interactable target.
    /// </summary>
    public class DialogueSystemTriggerInteractableTarget : DialogueSystemTrigger, Opsive.UltimateCharacterController.Traits.IInteractableTarget
    {
        private Transform m_player = null;
        private Transform player
        {
            get
            {
                if (m_player == null)
                {
                    var go = GameObject.FindGameObjectWithTag("Player");
                    if (go != null) m_player = go.transform;
                }
                return m_player;
            }
        }

        public bool CanInteract()
        {
            return true;
        }

        public void Interact()
        {
            if (player != null)
            {
                OnUse(player.transform);
            }
            else
            {
                OnUse();
            }
        }

    }
}