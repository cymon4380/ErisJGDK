using ErisJGDK.Base;
using ErisJGDK.Base.UI;
using UnityEngine;

namespace ErisJGDK.Example
{
    public class TestActions : MonoBehaviour
    {
        public void AnimateScore()
        {
            foreach(PlayerCard card in FindObjectsOfType<PlayerCard>())
            {
                if (card.Player == null)
                    continue;

                int previousScore = card.Player.Score;
                card.Player.Score = Random.Range(0, 10000);

                card.CountUpScore(previousScore, card.Player.Score);
            }
        }

        public void DropAllInputs()
        {
            RoomManager.Instance.CurrentRoom.DropInputs(RoomManager.Instance.CurrentRoom.Players.ToArray());
        }

        public void ShowLogo()
        {
            RoomManager.Instance.CurrentRoom.ShowLogo(RoomManager.Instance.CurrentRoom.Players.ToArray());
        }

        public void SendTextField()
        {
            Inputs inputs = new("Test Field", new()
            {
                new InputText("testInputField", 48)
            }, true);

            RoomManager.Instance.CurrentRoom.SendInputs(inputs, RoomManager.Instance.CurrentRoom.Players.ToArray());
        }

        public void SendButtonChoice()
        {
            Inputs inputs = new("Do you like this game?", new()
            {
                new InputButton("likeGame", "Yes", "yes1"),
                new InputButton("likeGame", "Certainly!", "yes2"),
                new InputButton("likeGame", "No", "no", disabled: true)
            }, false);

            RoomManager.Instance.CurrentRoom.SendInputs(inputs, RoomManager.Instance.CurrentRoom.Players.ToArray());
        }

        public void SendRange()
        {
            Inputs inputs = new("What salary (in US dollars) would you like to have?", new()
            {
                new InputRange("salary", 1000, 100000, 500, value: 7500)
            }, true);

            RoomManager.Instance.CurrentRoom.SendInputs(inputs, RoomManager.Instance.CurrentRoom.Players.ToArray());
        }
    }
}