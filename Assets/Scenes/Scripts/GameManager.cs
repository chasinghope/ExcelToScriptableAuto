using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public GameConfigSO gameConfig;
        private void Start()
        {
            foreach (var item in gameConfig.Item.Elements)
            {
                Debug.Log($"{item.ID} - {item.Name} - {item.Prop}");
            }
        }
    }
}