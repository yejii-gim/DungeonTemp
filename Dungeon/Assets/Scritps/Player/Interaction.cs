using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;
    public GameObject curInteractGameObject;
    private IInteractable curInteractable;
    private ItemData curItemObject;
    [SerializeField] private Transform _thirdPersonTransform; // 3인치 전용 Ray 시작 위치
    [SerializeField] TMP_Text _rayText;
    [SerializeField] ItemData[] items;
    public TextMeshProUGUI promptText;
    private Camera camera;


    private void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            Ray ray;
            if (CharcterManager.Instance.player.controller.isFirstPerson)
            {
                camera = CharcterManager.Instance.player.controller.firstPerson;
                ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                _rayText.text = "1인칭 시점";
            }
            else
            {
                // 캐릭터의 앞을 기준으로 Ray 생성
                camera = CharcterManager.Instance.player.controller.thirdPerson;
                ray = new Ray(_thirdPersonTransform.position, camera.transform.forward);
                _rayText.text = "3인칭 시점";
            }
           
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * maxCheckDistance, Color.red);
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    curItemObject = hit.collider.GetComponent<ItemObject>().ItemData;
                    // 프롬포트에 출력
                    SetPromptText();
                    Debug.Log(curItemObject.type);
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null)
        {
            if (curItemObject.type == ItemType.Box)
            {
                int idx = Random.Range(0, items.Length);
                InventoryManager.Instance.ThrowItem(items[idx]);
            }
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
            curItemObject = null;
        }
    }
}
