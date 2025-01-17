using Godot;

namespace IntoTheCrypt.Doors
{
    public abstract class DoorController : Node
    {
        #region Public

        #region Members
        public Node Door;
        public bool IsInteractable { get; set; }
        public bool IsOpen { get; set; } = false;
        #endregion

        #endregion

        #region Protected

        #region Member Methods
        protected abstract bool CanPlayerInteract();

        /*protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.PLAYER_INTERACTOR_TAG))
            {
                IsInteractable = CanPlayerInteract();
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.PLAYER_INTERACTOR_TAG))
            {
                IsInteractable = false;
            }
        }*/

        protected abstract void Interact();

        public override void _Process(float delta)
        {
            /*if (IsInteractable && Input.GetKeyUp(KeyCode.Return))
            {                
                Interact();
            }*/
        }
        #endregion

        #endregion
    }
}