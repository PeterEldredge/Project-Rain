using UnityEngine;
using PixelCrushers.DialogueSystem;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.StateSystem;
using Opsive.UltimateCharacterController.Camera;

namespace Opsive.UltimateCharacterController.Character.Abilities
{
    /// <summary>
    /// The Converse ability runs when a character is the primary actor or conversant
    /// in an active conversation.
    /// </summary>
    [DefaultStartType(AbilityStartType.Manual)]
    [DefaultStopType(AbilityStopType.Manual)]
    [DefaultInputName("Action")]
    [DefaultAbilityIndex(-1)]
    [DefaultAllowPositionalInput(false)]
    [DefaultAllowRotationalInput(false)]
    [DefaultEquippedSlots(0)]
    public class Converse : Ability
    {
        [Tooltip("Hide UCC UI when this character is conversing.")]
        [SerializeField] protected bool m_HideUI = true;

        [Tooltip("Disable gameplay input when this character is conversing.")]
        [SerializeField] protected bool m_DisableGameplayInput = true;

        [Tooltip("Detach Opsive camera control when this character is conversing.")]
        [SerializeField] protected bool m_DetachCamera = true;

        /// <summary>
        /// Add a Dialogue System Events component that listens for OnConversationStart and
        /// OnConversationEnd events to start and stop this ability.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            var dialogueSystemEvents = m_GameObject.AddComponent<DialogueSystemEvents>();
            dialogueSystemEvents.conversationEvents.onConversationStart.AddListener((Transform actor) => { StartAbility(); });
            dialogueSystemEvents.conversationEvents.onConversationEnd.AddListener((Transform actor) => { StopAbility(); });
        }

        protected override void AbilityStarted()
        {
            base.AbilityStarted();
            if (m_HideUI)
            {
                EventHandler.ExecuteEvent(m_GameObject, "OnShowUI", false);
            }
            if (m_DisableGameplayInput)
            {
                EventHandler.ExecuteEvent(m_GameObject, "OnEnableGameplayInput", false);
            }
            if (m_DetachCamera)
            {
                var lookSource = m_CharacterLocomotion.LookSource;
                if (lookSource != null)
                {
                    var cameraController = lookSource.GameObject.GetComponent<CameraController>();
                    if (cameraController != null)
                    {
                        cameraController.enabled = false;
                    }
                }
            }
            StateManager.SetState(m_GameObject, "Conversing", true);
        }

        protected override void AbilityStopped(bool force)
        {
            base.AbilityStopped(force);
            if (m_HideUI)
            {
                EventHandler.ExecuteEvent(m_GameObject, "OnShowUI", true);
            }
            if (m_DisableGameplayInput)
            {
                EventHandler.ExecuteEvent(m_GameObject, "OnEnableGameplayInput", true);
            }
            if (m_DetachCamera)
            {
                var lookSource = m_CharacterLocomotion.LookSource;
                if (lookSource != null)
                {
                    var cameraController = lookSource.GameObject.GetComponent<CameraController>();
                    if (cameraController != null)
                    {
                        cameraController.enabled = true;
                    }
                }
            }
            StateManager.SetState(m_GameObject, "Conversing", false);
        }

    }
}
