# 🏰 던전 (Dungeon)

- **게임명** : 던전
- **장르** : Unity 기반 3D 액션 게임
- **개발 엔진**: Unity `2022.3.17f1`
- **개발 기간**: 2025.05.16 ~ 2025.05.23

## 🎮 주요 기능

### ❤️‍🔥 체력 UI & 레이저
 - 레이저나 몬스터에 닿으면 체력이 감소하며 UI가 실시간으로 갱신
 - 체력이 0이 되면 사망
<details>
<summary>🔽 체력 UI & 레이저 코드 및 GIF 보기</summary>
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
<details>
<summary> 🔽 동적 환경 조사 코드 및 GIF 보기 </summary>
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
 - 특정 플랫폼에 플레이어가 닿으면 점프 또는 이동 기능이 활성화
 - 점프대는 플레이어를 위로 튕겨내고 이동형 플랫폼은 플레이어를 함께 이동
<details>
<summary>🔽  플랫폼들  코드 및 GIF 보기</summary>
  
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
<summary>🔽 아이템 데이터 & 아이템 사용 및 GIF 보기</summary>
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

---

### 🎨 추가 UI
 - 스킬 사용시 소모되는 스태미나 바, 허기 , 조작법 알려주는 UI 추가 구현
<details>
<summary> 🔽  추가 UI 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/731258b7-0e6e-484c-8fb9-c8afdf80f3a8" alt="추가 UI" width="600"/>
</div>

  ### 1️⃣ 스태미나 바

  ```csharp
Condition stamina { get { return uiCondition.stamina; } }
private void Update()
{
    stamina.Add(stamina.passiveValue * Time.deltaTime);
}

public bool UseStamina(float amount)
{
    if(stamina.curValue - amount < 0f)
    {
        return false;
    }
    stamina.Substact(amount);
    return true;
}
```
---

### 2️⃣ 허기 바
```csharp
Condition hunger { get { return uiCondition.hunger; } }
private void Update()
{
    if (!isInvincible)
    {
        hunger.Substact(hunger.passiveValue * Time.deltaTime);

        if (hunger.curValue == 0f) 
        {
            health.Substact(noHungerHealthDecay * Time.deltaTime);
        }
    }
}

public void Eat(float count)
{
    hunger.Add(count);
}
```

### 3️⃣ 조작법 알려주는 UI
```csharp
 public GameObject informationWindow;
 private void Start()
 {
     var controller = CharcterManager.Instance.player.controller ?? CharcterManager.Instance.player.GetComponent<PlayerController>();
     controller.OnInformation += Toggle;
     informationWindow.SetActive(false);
 }

 public void Toggle()
 {
     UIManager.Instance.Toggle(informationWindow);
 }
```

</details>

---

### 🔄 시점 전환
 - 1인칭 & 3인칭 카메라 모드 전환 가능
<details>
<summary>🔽  시점 전환 코드 및 GIF 보기</summary>
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

---

### 🧗 벽타기
 - 플레이어가 벽에 가까이 있을 때 점프 버튼을 누르면 벽타기 가능능
<details>
<summary> 🔽  벽타기 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/1aefe1be-4c20-4b39-bd18-91aa3949188a" alt="벽타기" width="600"/>
</div>

  ### 1️⃣ 벽인지 체크(Raycast)

  ```csharp
 private bool CheckWall()
 {
     Ray ray = new Ray(transform.position, transform.forward);
     _wallCheck = Physics.Raycast(ray, _climbCheckDistance, wallLayer);
     if (_wallCheck) return true;
     return false;
 }
```
---

### 2️⃣ 점프 입력시 벽 타기 조건 추
```csharp
public void OnJump(InputAction.CallbackContext context)
{
    if (context.phase != InputActionPhase.Started) return;

    if(IsGrounded()) // 점프
    {
         _rb.AddForce(Vector2.up * _jumpPower, ForceMode.Impulse);
    }
    else if(CheckWall() && !IsGrounded()) // 벽타기
    {
        _rb.AddForce(Vector3.up * _jumpPower * 5f, ForceMode.Impulse);
    }
}
```

</details>

---

### 🧾 다양한 아이템 구현
 - 아이템 습득시 스킬(대쉬, 무적, 더블 점프) 해금 가능 
<details>
<summary> 🔽  다양한 아이템 코드 UI 코드 및 GIF 보기</summary>

  ### 1️⃣ 대쉬
  
<div align="center">
<img src="https://github.com/user-attachments/assets/85dadb98-b52a-4151-8564-de1fa466dac4" alt="대쉬" width="600"/>
</div>

  ```csharp
public void OnDash(InputAction.CallbackContext context)
{
    if (context.phase == InputActionPhase.Started)
    {
        if (SkillManager.Instance.CheckUnLockSkill(SkillType.Dash) && !isDash)
        {
            SkillManager.Instance.TriggerCooldown(SkillType.Dash);
            condition.Dash(20f);
            StartCoroutine(Dash(_dashPower));
        }
    }
}

private IEnumerator Dash(float dashPower)
{
    isDash = true;
    Camera cam = CharcterManager.Instance.player.controller.isFirstPerson ? CharcterManager.Instance.player.controller.firstPerson : CharcterManager.Instance.player.controller.thirdPerson;
    Vector3 dir = cam.transform.forward;
    dir.y = 0f; // 수평이동만 하기위해 y를 0으로 설정
    dir.Normalize();
    _rb.AddForce(dir * dashPower, ForceMode.Impulse);
    CharcterManager.Instance.player.controller.canMove = false;
    Invoke(nameof(CharcterManager.Instance.player.controller.EnableMove), 0.5f);
    yield return new WaitForSeconds(SkillManager.Instance.GetCoolTime(SkillType.Dash));

    isDash = false;
}
```
---

### 2️⃣ 더블 점프

<div align="center">
<img src="https://github.com/user-attachments/assets/d432f08e-c4b6-40ce-9b9d-ddbc43027e34" alt="더블 점프" width="600"/>
</div>

```csharp
public void OnDoubleJump(InputAction.CallbackContext context)
{
    if (context.phase == InputActionPhase.Started)
    {
        if (IsGrounded() && !isDoubleJump)
        {
            jumpCount = 0;
            isDoubleJump = true;
            SkillManager.Instance.TriggerCooldown(SkillType.DoubleJump);
            condition.DoubleJump(10f);
        }
        if (SkillManager.Instance.CheckUnLockSkill(SkillType.DoubleJump) && jumpCount < maxJumpCount && isDoubleJump)
        {
            jumpCount++;
            _rb.velocity = new Vector2(_rb.velocity.x, 0f); 
            _rb.AddForce(Vector2.up * _jumpPower, ForceMode.Impulse);
            if(jumpCount == maxJumpCount) isDoubleJump = false;
        }
    }
}
```

### 3️⃣ 무적

<div align="center">
<img src="https://github.com/user-attachments/assets/a6f7c478-e6fd-410f-bb6a-9e73c97a5dcd" alt="무적" width="600"/>
</div>

```csharp
// PlayerCondition.cs
 private void Update()
 {
     stamina.Add(stamina.passiveValue * Time.deltaTime);
     if (!isInvincible)
     {
         hunger.Substact(hunger.passiveValue * Time.deltaTime);
         

         if (hunger.curValue == 0f)
         {
             health.Substact(noHungerHealthDecay * Time.deltaTime);
         }

         if (health.curValue == 0f)
         {
             Die();
         }
     }
     // 무적 관련
     else
     {
         invincibleTime -= Time.deltaTime;
         if (invincibleTime <= 0f)
         {
             isInvincible = false;  
         }
     }
 }

// PlayerController.cs
public void OnInvincible(InputAction.CallbackContext context)
{
    if (context.phase == InputActionPhase.Started)
    {
        if (SkillManager.Instance.CheckUnLockSkill(SkillType.Invincible))
        {
            SkillManager.Instance.TriggerCooldown(SkillType.Invincible);
            condition.Invincibility(40f, SkillManager.Instance.GetCoolTime(SkillType.Invincible));
        }
    }    
}
```

</details>

---

### ⚒️ 장비 장착 및 해제

<details>
<summary> 🔽 장비 장착 및 해제 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/ff685e21-5b59-4e3b-a850-6455b14d72a9" alt="장비 장착 및 해제" width="600"/>
</div>

  ### 1️⃣ 장비 장착
  
  ```csharp
// 아이템 선택시 상세정보 UI에 표시하게 해주는 함수
public void SelectItem(int index)
{
    if (slots[index].item == null) return;

    selectedItem = slots[index].item;
    selectedItemIndex = index;

    // 텍스트 정보 업데이트
    selectedItemName.text = selectedItem.displayName;
    selectedItemDescription.text = selectedItem.description;

    selectedStatName.text = string.Empty;
    selectedStatValue.text = string.Empty;

    // 소비 아이템 효과 목록 표시
    for (int i = 0; i < selectedItem.consumables.Length; i++)
    {
        selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
        selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
    }

    // 버튼 상태 설정
    useButton.SetActive(selectedItem.type == ItemType.Consumable);
    equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
    unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
    dropButton.SetActive(true);
}

public void OnEquipButton()
{
    if (slots[curEquipIndex].equipped)
    {
        UnEquip(curEquipIndex);
    }
    slots[selectedItemIndex].equipped = true;
    curEquipIndex = selectedItemIndex;
    CharcterManager.Instance.player.equipment.EquipNew(selectedItem);
    InventoryManager.Instance.UpdateUI();

    SelectItem(selectedItemIndex);
}
```
---

### 2️⃣ 장비 해제
```csharp
 public void OnEquipButton()
{
    if (slots[curEquipIndex].equipped)
    {
        UnEquip(curEquipIndex);
    }
    slots[selectedItemIndex].equipped = true;
    curEquipIndex = selectedItemIndex;
    CharcterManager.Instance.player.equipment.EquipNew(selectedItem);
    InventoryManager.Instance.UpdateUI();

    SelectItem(selectedItemIndex);
}

public void OnUnEquipButton(int index)
{
    UnEquip(index);
}

void UnEquip(int index)
{
    slots[index].equipped = false;
    CharcterManager.Instance.player.equipment.UnEquip();
    InventoryManager.Instance.UpdateUI();

    if (selectedItemIndex == index)
    {
        SelectItem(selectedItemIndex);
    }
}

```

</details>

---

### 📦 상호작용 가능한 오브젝트 표시
 - 플레이어가 상호작용 가능한 오브젝트(Box 등) 에 접근하면 UI 표시
 - Box를 열면 랜덤으로 아이템 드롭
<details>
<summary> 🔽 상호작용 가능한 오브젝트 표시 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/a92b2485-481c-4d9c-ab90-943d0376506a" alt="상호작용 가능한 오브젝트 표시" width="600"/>
</div>
 
  ### 1️⃣ 상호작용 입력 처리
  
```csharp
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
        UIManager.Instance.ClosePrompt();
        curItemObject = null;
    }
}
```


</details>

---

### 🚀 플랫폼 발사기
 - 플레이어가 플랫폼 위에 서 있을 때 특정 방향으로 힘을 가해 발사하는 시스템 구현
 - 특정 키를 누르면 발
<details>
<summary> 🔽 플랫폼 발사 코드 및 GIF 보기</summary>
<div align="center">
<img src="https://github.com/user-attachments/assets/8d8fd3ae-0805-46f3-9163-42a914465436" alt="플랫폼 발사기" width="600"/>
</div>
 
  ### 1️⃣ 물리 기반 발사 로직
  
```csharp
private void Shoot()
{
    Vector3 dir =  shootDirection.position - shootPosition.position;
    _rb.AddForce(dir * _shootPower, ForceMode.Impulse);
    CharcterManager.Instance.player.transform.rotation = _orgRotation;
    CharcterManager.Instance.player.controller.canMove = false;
    Invoke(nameof(CharcterManager.Instance.player.controller.EnableMove), 0.5f);
}
```

---
### 2️⃣ 상호작용 입력 처리
```csharp
 public void OnShoot(InputAction.CallbackContext context)
 {
     if (context.phase == InputActionPhase.Started && curInteractable != null)
     {
         if (curInteractable is Shooter shooter)
         {
             curInteractable.OnInteract();
         }
        
     }
 }
```

</details>

---

## 🛠️ 트러블슈팅
1. AddForce를 사용해서 발사기를 구현하였는데 발사되지 않는 문제
Move의 veloctiy와 겹쳐서 AddForce가 실행되지 않았음 그래서 발사기를 사용할때는 Move를 멈추도록 설정

2. Dash 사용시 캐럭터가 보는 방향이 아니라 다른 방향으로 나가는 문제
플레이어 위치를 기반으로 방향을 정했더니 현재 보이는 위치의 앞으로 가지 않았음. 현재 시점의 카메라의 forwad 벡터 기반으로 설정

3. 시점 전환 후 RayCast가 이상하게 나가서 아이템들과 상호작용이 잘 안되는 문제
1인칭은 화면 중앙 기준, 3인칭은 카메라 전방 벡터 기반으로 Ray 분기 처리했더니 해결

