# 🏰 던전 (Dungeon)

> Unity 기반 3D 액션 게임

## **던전**은 Unity와 C#으로 제작된 3D 액션 게임입니다.  
### 플레이어는 다양한 시점(1인칭 / 3인칭)으로 전환하며, 상호작용, 스킬, 플랫폼, 적 처치 등의 메커니즘을 통해 탐험합니다.

## 🎮 주요 기능


### ❤️‍🔥 체력 UI & 레이저
 - 레이저나 몬스터에 닿으면 체력이 감소하며 UI가 실시간으로 갱싱된다.
 - 체력이 0이 되면 사망한다.
<details>
<summary>❤️‍🔥 체력 UI & 레이저 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/9c01e4b7-c53d-4363-b287-ddf675a8d31e" alt="체력 UI 변화 및 레이저" width="600"/>
</div>

  ### 1️⃣ 사망 및 데미지 처리

  ```csharp
public void Die()
{
    Debug.Log("Die");
}

public void TakePhysicalDamage(int damage)
{
    // 무적 상태일시 무시
    if (isInvincible)
    {
        return;
    }
    health.Substact(damage);
    onTakeDamage?.Invoke();
}
```
---
### 2️⃣ 피격시 화면 플래
```csharp
 private void Start()
 {
     CharcterManager.Instance.player.condition.onTakeDamage += Flash;
 }

 // 피격시 호출되어 화면에 빨간색 보이게하는 함수
 public void Flash()
 {
     if(coroutine != null)
     {
         StopCoroutine(coroutine);
     }
     image.enabled = true;
     image.color = new Color(1f, 100f / 255f, 100f / 255f);
     coroutine = StartCoroutine(FadeAway());
 }
```
---
### 3️⃣ 레이저 충돌 체크 및 데미지 적용
```csharp
 private void Update()
{
    RaycastHit hit;
    bool isHit = false;
    if (Physics.Raycast(transform.position, transform.right, out hit, 10f))
    {
        if (hit.collider.CompareTag("Player"))
        {
            isHit = true;
            if (!isAttack)
            {
                isAttack = true;
                CharcterManager.Instance.player.condition.TakePhysicalDamage(1);
            }
            _laserLight.SetActive(true);
        }
    }
    if(!isHit) // 플레이어가 레이저에서 벗어난 상태
    {
        isAttack = false;
        _laserLight.SetActive(false);
    }
    Debug.DrawRay(transform.position, transform.right * 10f);
}
```

</details>

---

### 🔍 동적 환경 조사
 - Raycast를 통해 플레이어가 조사하는 오브젝트의 정보를 UI에 표시
 - 체력이 0이 되면 사망한다.
<details>
<summary>🔍 동적 환경 조사 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/2af629e1-7d63-4f76-8140-fffb1a21a281" alt="상호작용" width="600"/>
</div>
  
### 1️⃣ 상호작용 시스템을 위한 인터페이스 정의
  
  ```csharp
public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}
```
  ---
  ### 2️⃣ 카메라 기반 동적 환경 조사 & UI 프롬프트 출력

  ```csharp
private void Update()
{
    if (Time.time - lastCheckTime > checkRate)
    {
        lastCheckTime = Time.time;

        Ray ray;
        if (CharcterManager.Instance.player.controller.isFirstPerson)
        {
            camera = CharcterManager.Instance.player.controller.firstPerson;
            ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        }
        else
        {
            camera = CharcterManager.Instance.player.controller.thirdPerson;
            ray = new Ray(_thirdPersonTransform.position, camera.transform.forward);
        }

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
        {
            if (hit.collider.gameObject != curInteractGameObject)
            {
                curInteractGameObject = hit.collider.gameObject;
                curInteractable = hit.collider.GetComponent<IInteractable>();
                SetPromptText();
            }
        }
        else
        {
            curInteractGameObject = null;
            curInteractable = null;
            UIManager.Instance.ClosePrompt();
        }
    }
}

private void SetPromptText()
{
    if (curInteractable != null)
    {
        UIManager.Instance.OpenPrompt(curInteractable.GetInteractPrompt());
    }
}
```
</details>

---

###  🎈 점프대 플랫폼 & 이동형 플랫폼
 - 특정 플랫폼에 플레이어가 닿으면 점프 또는 이동 기능이 활성화된다.
 - 점프대는 플레이어를 위로 튕겨내고 이동형 플랫폼은 플레이어를 함께 이동시킨다.
<details>
<summary>🎈 플랫폼들  코드 및 GIF 보기</summary>
  
  ### 1️⃣ 점프 패드
<div align="center">
<img src="https://github.com/user-attachments/assets/8ab0884c-1980-4a84-a8db-92d388fb25a6" alt="점프대" width="600"/>
</div>

  ```csharp
private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
}
```
---
### 2️⃣ 이동형 플랫폼
<div align="center">
<img src="https://github.com/user-attachments/assets/2d91617d-e915-415a-bd2d-3299be911d18" alt="이동형 발판" width="600"/>
</div>

```csharp
private void Update()
 {
     if (!isActive) return; 

   // 오른쪽 방향으로 이동
   transform.position += Vector3.right * _speed * Time.deltaTime;
    // 지정된 거리만큼 이동하면 방향 거꾸로 지정
    if (Vector3.Distance(startPos, transform.position) >= _moveDistance)
    {
       _speed *= -1;
   }

 }

 void LateUpdate()
 {
     if (!isActive) return;

     Vector3 distance = transform.position - preivousPos;
     preivousPos = transform.position;

     // 플레이어도 이동
     CharcterManager.Instance.player.transform.position += distance;
 }
 private void OnCollisionEnter(Collision collision)
 {
     if (collision.collider.CompareTag("Player"))
     {
         isActive = true;
     }
 }

 private void OnCollisionExit(Collision collision)
 {
     if (collision.collider.CompareTag("Player"))
     {
         isActive = false;
     }
 }
```

</details>

---

### 🛒 아이템 데이터 & 아이템 사용
 - 다양한 아이템 데이터를 ScriptableObject로 정의. 각 아이템의 이름, 설명, 속성 등을 ScriptableObject로 관리
 - 소비 아이템 사용시 아이템 종류에 따라 효과 적용
<details>
<summary>🛒아이템 데이터 & 아이템 사용 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/e146b79a-e591-434e-a076-e09bdfea84e3" alt="아이템 사용" width="600"/>
</div>

  ### 1️⃣ 아이템 데이터(ScriptableObject)
  
  ```csharp
public enum ItemType
{
    Equipable,
    Consumable,
    Resource,
    Box
}
public enum ConsumableType
{
    Health,
    Hunger,
    Invincible,
    Dash,
    DoubleJump,
}
[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite Icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("ConumableType")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public GameObject equipPrefab;
}
```
---

### 2️⃣ 아이템 사용
```csharp
 public void OnUseButton()
 {
     if (selectedItem.type != ItemType.Consumable && selectedItem.type != ItemType.Box) return;
     
     foreach (var effect in selectedItem.consumables)
     {
         switch (effect.type)
         {
             case ConsumableType.Health: condition.Heal(effect.value); break;
             case ConsumableType.Hunger: condition.Eat(effect.value); break;
             case ConsumableType.Invincible:
                 SkillManager.Instance.UnLockSkill(SkillType.Invincible);
                 break;
             case ConsumableType.Dash:
                 SkillManager.Instance.UnLockSkill(SkillType.Dash);
                 break;
             case ConsumableType.DoubleJump:
                 SkillManager.Instance.UnLockSkill(SkillType.DoubleJump);
                 break;
         }
     }
     RemoveSelectedItem();
 }

```
</details>

### 🔄 추가 UI
 - 스킬 사용시 소모되는 스태미나 바, HUNGRY, 조작법 알려주는 ui 추가 구현
<details>
<summary>🔄 시점 전환 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/731258b7-0e6e-484c-8fb9-c8afdf80f3a8" alt="추가 UI" width="600"/>
</div>

  ### 1️⃣ 스태미나나

  ```csharp
public void onSwitchCamera(InputAction.CallbackContext context)
{
    if (context.phase == InputActionPhase.Started)
    {
        isFirstPerson = !isFirstPerson;
        if (isFirstPerson)
        {
            firstPerson.gameObject.SetActive(true);
            thirdPerson.gameObject.SetActive(false);
        }
        else
        {
            thirdPerson.gameObject.SetActive(true);
            firstPerson.gameObject.SetActive(false);
        }
    }
}
```
---

### 2️⃣ JUNGRY
```csharp
Ray ray;

if (CharcterManager.Instance.player.controller.isFirstPerson)
{
    camera = CharcterManager.Instance.player.controller.firstPerson;
    ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
    _rayText.text = "1인칭 시점";
}
else
{
    camera = CharcterManager.Instance.player.controller.thirdPerson;
    ray = new Ray(_thirdPersonTransform.position, camera.transform.forward);
    _rayText.text = "3인칭 시점";
}
```

### 3️⃣ 조작법 알려주는 ui
```csharp
Ray ray;

if (CharcterManager.Instance.player.controller.isFirstPerson)
{
    camera = CharcterManager.Instance.player.controller.firstPerson;
    ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
    _rayText.text = "1인칭 시점";
}
else
{
    camera = CharcterManager.Instance.player.controller.thirdPerson;
    ray = new Ray(_thirdPersonTransform.position, camera.transform.forward);
    _rayText.text = "3인칭 시점";
}
```

</details>


### 🔄 시점 전환
 - 1인칭 & 3인칭 카메라 모드 전환 가능
<details>
<summary>🔄 시점 전환 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/c8b400e7-b76c-4715-a3fe-28acdb751372" alt="3인칭 시점" width="600"/>
</div>

  ### 1️⃣ 시점 전환 입력 처리

  ```csharp
public void onSwitchCamera(InputAction.CallbackContext context)
{
    if (context.phase == InputActionPhase.Started)
    {
        isFirstPerson = !isFirstPerson;
        if (isFirstPerson)
        {
            firstPerson.gameObject.SetActive(true);
            thirdPerson.gameObject.SetActive(false);
        }
        else
        {
            thirdPerson.gameObject.SetActive(true);
            firstPerson.gameObject.SetActive(false);
        }
    }
}
```
---

### 2️⃣ 상호작용 시 시점에 따른 Ray 처리
```csharp
Ray ray;

if (CharcterManager.Instance.player.controller.isFirstPerson)
{
    camera = CharcterManager.Instance.player.controller.firstPerson;
    ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
    _rayText.text = "1인칭 시점";
}
else
{
    camera = CharcterManager.Instance.player.controller.thirdPerson;
    ray = new Ray(_thirdPersonTransform.position, camera.transform.forward);
    _rayText.text = "3인칭 시점";
}
```
</details>


![추가 UI](https://github.com/user-attachments/assets/731258b7-0e6e-484c-8fb9-c8afdf80f3a8)
![벽타기](https://github.com/user-attachments/assets/1aefe1be-4c20-4b39-bd18-91aa3949188a)
![아이템 사용](https://github.com/user-attachments/assets/e146b79a-e591-434e-a076-e09bdfea84e3)

