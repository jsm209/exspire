using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    #region Player Related Variables


    [Header("Main Player Stats")]
    // Modifiable Stats
    public int maxHealth; // current player max health
    public int health; // current player health
    public int maxKindles = 3; // Max amount player can hold. (clip size)
    public int kindles = 3; // Currency used for casting abilities. (current amount of kindles)
    public float baseProjectileForce = 20f; // Standard speed to fire projectiles
    public GameObject[] projectiles; // All the projectiles the player can switch between right now
    public float moveSpeed = 5f; // Speed at which to move at.
    public float rollSpeed = 250f; // Speed at which to roll at.
    public float rechargeDelay; // Seconds before charges begin to replenish.
    public float rechargeRate; // Seconds between replenishing a charge.
    public float invulnerabilitySeconds; // How many seconds after being hit that the player has iframes.






    [Header("Extra Player Stats")]

    public float timeLeft = 600; // Time left in game.
    public float timeSurvived = 0; // Total time survived in game
    public int currentFloor = 1; // Current floor the player is on
    public bool[] defeatedBosses = { false, false, false, false, false }; // Tells us which bosses player defeated.
    public int kills; // Total amount of kills by the player
    public int[] inventory;
    public bool loadDefaultPlayerStats; // Whether or not the game should load defaault stats

    [Header("Other")]
    // UI Related
    public bool shouldLoadPlayerData; // Tells the game whether or not to load the player's data.
    public bool inTutorialLevel; // If the player runs out of time in a tutorial level, it simply resets.

    private float timeSinceLastAbility; // The time passed since we last used some kindles. Used for tracking initial inactivity to tell when to begin auto regen
    private bool isInvulnerable; // Tracks if the player is able to take damage
    private bool hasFloorKey; // Does the player currently hold the floor key
    #endregion

    #region UI Variables
    public TextMeshProUGUI timerText;
    public Image[] kindleContainer;
    public Sprite fullKindle;
    public Sprite emptyKindle;
    public GameObject healthBar;
    public GameObject energyBar;
    public GameObject fadeBlack;
    #endregion


    // Code related private variables
    private Animator anim;

    // Saves the player's data
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    // Loads the stored player data
    public void LoadPlayer()
    {
        // Loads and sets player data stored within this player

        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null)
        {
            return;
        }

        if (loadDefaultPlayerStats)
        {
            maxHealth = 3;
            health = 3;
            maxKindles = 6;
            kindles = 6;
            baseProjectileForce = 40;
            moveSpeed = 150;
            rollSpeed = 250;
            rechargeDelay = 0.5f;
            rechargeRate = 1;
            invulnerabilitySeconds = 1;
            defeatedBosses = data.defeatedBosses;
            timeLeft = 600;
            currentFloor = 1;

            // Creating an empty inventory
            inventory = new int[50];

            // Initial records:
            timeSurvived = 0.0f;
            kills = 0;


        }
        else
        {
            maxHealth = data.maxHealth;
            health = data.health;
            maxKindles = data.maxKindles;
            kindles = data.maxKindles;
            baseProjectileForce = data.baseProjectileForce;
            moveSpeed = data.moveSpeed;
            rollSpeed = data.rollSpeed;
            rechargeDelay = data.rechargeDelay;
            rechargeRate = data.rechargeRate;
            invulnerabilitySeconds = data.invulnerabilitySeconds;
            defeatedBosses = data.defeatedBosses;
            timeLeft = data.timeLeft;
            currentFloor = data.currentFloor;

            inventory = data.inventory;

            timeSurvived = data.timeSurvived;
            kills = data.kills;

            projectiles = getProjectilesFromIDs(data.projectiles);
        }

        Debug.Log(inventory.Length);

        UpdateAbilityMovementVariables();

        // Could also restore position, but mostly unused because each room/map is different.
        /*
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
        */
    }

    // Adds to the kindle count.
    public void addKindles(int n)
    {
        kindles += n;
        timeSinceLastAbility = 0.0f;
    }

    // Adds to the timeLeft count.
    public void addTime(int n)
    {
        timeLeft += n;
    }


    public void addMaxKindles(int n)
    {
        maxKindles += n;
    }

    void Awake()
    {
        isInvulnerable = false;
        hasFloorKey = false;

        if (shouldLoadPlayerData)
        {
            this.LoadPlayer();
        }
    }

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        updateTimerDisplay();
        updateKindleDisplay();
        updateHealthDisplay();
        updateFloorKeyDisplay();


        // Update time related stats only if the player is alive
        if (this.health > 0) {
            timeSurvived += 1 * Time.deltaTime;
            timeLeft -= 1 * Time.deltaTime;
        }
        

        // Checks if timer hits zero. If it does, start a boss encounter.

        if (timeLeft <= 0)
        {
            if (inTutorialLevel) {
                StartCoroutine(changeScene("TutorialDungeon"));
            } else {
                Death();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                Debug.Log(i + ", " + inventory[i]);
            }
        }
    }

    void FixedUpdate()
    {
        checkAndReplenishKindles();
    }

    // Given an integer presenting the total seconds
    // Will return a string of the time in 00:00 format.
    public string formatTime(float totalSeconds) {
        int minutes = (int)totalSeconds / 60;
        int seconds = (int)totalSeconds % 60;

        string text = "";

        if (seconds < 10) {
            text = minutes.ToString() + " : " + "0" + seconds.ToString();
        } else {
            text = minutes.ToString() + " : " + seconds.ToString();
        }

        return text;

    }

    // Updates a text object to display the appropriate amount of
    // minutes and seconds from total seconds.
    void updateTimerDisplay()
    {
        timerText.text = formatTime(timeLeft);
    }

    void checkAndReplenishKindles()
    {
        timeSinceLastAbility += 1 * Time.deltaTime;
        if (timeSinceLastAbility > rechargeDelay + rechargeRate)
        {
            timeSinceLastAbility = rechargeDelay;
            if (kindles < maxKindles)
            {
                kindles += 1;
            }

        }
    }

    void updateFloorKeyDisplay()
    {
        GameObject.FindWithTag("FloorKeyImage").GetComponent<Image>().enabled = this.hasFloorKey;
    }

    void updateHealthDisplay()
    {
        this.healthBar.GetComponent<Healthbar>().SetHealth(this.health);
        this.healthBar.GetComponent<Healthbar>().SetMaximumHealth(this.maxHealth);
    }

    // Shows the right amount of full and empty kindles.
    void updateKindleDisplay()
    {
        this.energyBar.GetComponent<Healthbar>().SetHealth(this.kindles);
        this.energyBar.GetComponent<Healthbar>().SetMaximumHealth(this.maxKindles);
        /*
        if (kindles > maxKindles)
        {
            kindles = maxKindles;
        }

        for (int i = 0; i < kindleContainer.Length; i++)
        {

            if (i < kindles)
            {
                kindleContainer[i].sprite = fullKindle;
            }
            else
            {
                kindleContainer[i].sprite = emptyKindle;
            }

            if (i < maxKindles)
            {
                kindleContainer[i].enabled = true;
            }
            else
            {
                kindleContainer[i].enabled = false;
            }
        }
        */

    }

    // Will force change the variables in the movement and abilities script
    // to update to those in this script.
    public void UpdateAbilityMovementVariables()
    {
        this.GetComponent<PlayerMovement>().setMoveSpeed(this.moveSpeed);
        this.GetComponent<PlayerMovement>().setRollSpeed(this.rollSpeed);
    }

    // Triggers are used for item pickups so that the collision doesn't
    // interrupt player movement.
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Money")
        {
            this.timeLeft++;

            if (inventory[18] > 0)
            {
                this.timeLeft += inventory[18];
            }

            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "FloorKey")
        {
            this.hasFloorKey = true;
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "Heart")
        {
            this.health++;
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "ProjectilePickup") {
            PlayerAbilities pa = this.gameObject.GetComponentInChildren<PlayerAbilities>();
            GameObject newProjectile = col.gameObject.GetComponent<ProjectilePickup>().getProjectile();
            pa.replaceCurrentProjectile(newProjectile);
            pa.updateProjectileDisplay(newProjectile);
            Destroy(col.gameObject);
        }
    }

    // Checks for all collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isInvulnerable)
        {
            if (col.gameObject.tag == "EnemyProjectile")
            {

                int damage = col.gameObject.GetComponent<Projectile>().getDamage();
                takeDamage(damage);
                StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .8f));
                this.GetComponent<TimeStop>().StopTime(0.3f, 20, 0.1f);
                StartCoroutine("Invulnerable");
            }
            else if (col.gameObject.tag == "Hitbox")
            {
                Debug.Log("hitbox triggered");

                StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .8f));
                this.GetComponent<TimeStop>().StopTime(0.3f, 20, 0.1f);

                Hitbox hitbox = col.gameObject.GetComponent<Hitbox>();

                takeDamage(hitbox.damage);
                doKnockback(col.gameObject, hitbox.knockback);
                StartCoroutine("Invulnerable");
            }
            else if (col.gameObject.tag == "Enemy")
            {
                Debug.Log("hurt by enemy");
                StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .8f));
                this.GetComponent<TimeStop>().StopTime(0.3f, 20, 0.1f);
                takeDamage(1);
                StartCoroutine("Invulnerable");
            }


        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!isInvulnerable)
        {
            if (col.gameObject.tag == "Hitbox")
            {


                Hitbox hitbox = col.gameObject.GetComponent<Hitbox>();
                if (hitbox.active)
                {
                    Debug.Log("hitbox triggered");
                    StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .8f));
                    this.GetComponent<TimeStop>().StopTime(0.3f, 20, 0.1f);
                    takeDamage(hitbox.damage);
                    doKnockback(col.gameObject, hitbox.knockback);
                    StartCoroutine("Invulnerable");
                }
            }
        }

    }

    public bool getFloorKey()
    {
        return this.hasFloorKey;
    }

    public void useFloorKey()
    {
        this.hasFloorKey = false;
    }

    void doKnockback(GameObject other, float force)
    {
        Vector3 knockbackDirection = this.transform.position - other.transform.position;
        this.GetComponent<Rigidbody2D>().AddForce(Vector3.Normalize(knockbackDirection) * force, ForceMode2D.Impulse);
    }

    // Makes the player unable to collide with "Enemy" or "EnemyProjectile" layer, as well as turn 
    // partially transparent for a given amount of time.
    IEnumerator Invulnerable()
    {
        isInvulnerable = true;
        Physics2D.IgnoreLayerCollision(9, 10, true); // Ignore all enemy projectiles
        Physics2D.IgnoreLayerCollision(9, 11, true); // Ignore all enemies
        gameObject.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        yield return new WaitForSeconds(invulnerabilitySeconds);
        gameObject.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Physics2D.IgnoreLayerCollision(9, 10, false);
        Physics2D.IgnoreLayerCollision(9, 11, false);
        isInvulnerable = false;
        yield return null;
    }

    public void takeDamage(int n)
    {
        if (this.health - n <= 0)
        {
            this.health = 0;
            Death();
        }
        else
        {
            this.health -= n;
        }

        if (inventory[16] > 0)
        {
            PlayerAbilities playerAbilities = this.GetComponentInChildren<PlayerAbilities>();
            for (int i = 0; i < inventory[16]; i++)
            {
                playerAbilities.ShootRandomDirection();
            }
        }
    }

    public void addKillCount(int n) {
        this.kills += n;
    }

    // Upon dying, disable player movement, 
    public void Death()
    {
        this.health = 0;
        if (inventory[15] > 0)
        {
            health = Mathf.FloorToInt(0.5f * maxHealth);
            inventory[15]--;
            GameObject.FindWithTag("Inventory").GetComponent<InventoryUI>().displayInventory();
            Debug.Log("USED CHRONO'S BUG");
        }
        else
        {
            this.anim.ResetTrigger("teleport");
            this.anim.SetTrigger("teleport");
            this.GetComponent<PlayerMovement>().setState("Dead");
            Debug.Log("PLAYER DEAD");
        }


        //Image overlay = GameObject.FindWithTag("ScreenFlash").GetComponent<Image>();
        //StartCoroutine(FadeIn(overlay, 0.9f));
    }

    IEnumerator FadeIn(Image image, float FadeRate)
    {
        float targetAlpha = 1.0f;
        Color curColor = image.color;
        while (Mathf.Abs(curColor.a - targetAlpha) > 0.1f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }
        StartCoroutine(FadeToBlack(image, 0.9f));


    }

    IEnumerator FadeToBlack(Image image, float FadeRate)
    {
        float black = 0f; ;
        Color curColor = image.color;
        while (Mathf.Abs(curColor.r - black) > 0.1f)
        {
            curColor.r = Mathf.Lerp(curColor.r, black, FadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // Will transport the player to a new scene.
    // Plays the teleport animation and changes the scene.
    public IEnumerator changeScene(string sceneName)
    {
        this.anim.ResetTrigger("teleport");
        this.anim.SetTrigger("teleport");
        fadeBlack.GetComponent<FadeBlack>().showBlack = true;
        yield return new WaitForSeconds(1.5f);
        //this.currentFloor += 1;
        //this.timeLeft = 600;
        this.SavePlayer();
        SceneManager.LoadScene(sceneName);
    }

    
    // Will start the tutorial level
    // Plays the teleport animation and changes the scene.
    public IEnumerator tutorialScene()
    {
        this.anim.ResetTrigger("teleport");
        this.anim.SetTrigger("teleport");
        fadeBlack.GetComponent<FadeBlack>().showBlack = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("TutorialDungeon");
    }

    // Will use the player's projectiles and return an array of those projectile IDs
    public int[] getPlayerProjectileIDs() {
        ProjectileDatabase pd = GameObject.FindWithTag("ProjectileDatabase").GetComponent<ProjectileDatabase>();
        if (pd == null) {
            Debug.LogError("ProjectileDatabase is missing from scene");
        }

        int[] ids = new int[this.projectiles.Length];
        int idsIndex = 0;
        for (int i = 0; i < this.projectiles.Length; i++) {
            if (idsIndex < ids.Length) {
                ids[idsIndex] = (pd.getIdOfProjectile(this.projectiles[i]));
            } else {
                return ids;
            }
        }
        return ids;
    }

    public GameObject[] getProjectilesFromIDs(int[] ids) {
        ProjectileDatabase pd = GameObject.FindWithTag("ProjectileDatabase").GetComponent<ProjectileDatabase>();

        if (ids == null || ids.Length == 0) {
            GameObject[] defaultProjectiles = new GameObject[1];
            defaultProjectiles[0] = pd.getProjectile(0);
            return defaultProjectiles;
        } else {
            GameObject[] newProjectiles = new GameObject[ids.Length];
            for (int i = 0; i < ids.Length; i++) {
                newProjectiles[i] = pd.getProjectile(ids[i]);
            }
            return newProjectiles;
        }
    }
}
