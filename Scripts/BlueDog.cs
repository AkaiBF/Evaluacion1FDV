using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueDog : MonoBehaviour
{

    public enum Status
    {
        Normal,
        Euphoria,
        Sad
    }

    public Sprite fullheart;
    public Sprite halfheart;

    public Canvas HUD;
    public Canvas EndGame;
    public Text EndGameTxt;

    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Text txt;

    public Status estado;
    public int vidas;
    public int puntuacion;
    public float jumpForce;
    public float moveSpeed;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform tr;
    private bool isGrounded;
    private bool running;
    private float dmgCD = 2.0f;
    private float celebrationForce = 200f;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true;
        this.estado = Status.Normal;
        this.vidas = 3;
        this.anim = this.GetComponent<Animator>();
        this.rb = this.GetComponent<Rigidbody2D>();
        this.tr = this.GetComponent<Transform>();
        running = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            // Salto
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Sólo salta si está tocando el suelo
                if (isGrounded)
                {
                    rb.AddForce(Vector2.up * jumpForce);
                    isGrounded = false;
                }
            }
            // Derecha
            if (Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.RightArrow))
            {
                if (this.GetComponent<SpriteRenderer>().flipX)
                {
                    this.GetComponent<SpriteRenderer>().flipX = false;
                }
                tr.Translate(Vector2.right * Time.deltaTime * moveSpeed);
                
            }
            // Izquierda
            if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (!this.GetComponent<SpriteRenderer>().flipX)
                {
                    this.GetComponent<SpriteRenderer>().flipX = true;
                }
                tr.Translate(Vector2.left * Time.deltaTime * moveSpeed);
                
            }
            // Boca arriba
            if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.DownArrow))
            {
                this.GetComponent<SpriteRenderer>().flipY = !this.GetComponent<SpriteRenderer>().flipY;
            }
            if (dmgCD > 0.0f)
            {
                // Bajamos cooldown hasta resetear daño
                dmgCD -= Time.deltaTime;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        if(collision.gameObject.name == "Caseta")
        {
            // Win condition
            running = false;
            Won();
        }
        if (collision.gameObject.name == "DZ")
        {
            // Win condition
            running = false;
            Lost();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (running)
        {
            // Recolección de hueso pequeño
            if (collision.gameObject.CompareTag("Hueso"))
            {
                puntuacion += 5;
                txt.text = "Puntuación: " + puntuacion;
                anim.SetTrigger("Happy");
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                Celebrate();
            }
            // Recolección de hueso grande
            if (collision.gameObject.CompareTag("HuesoGrande"))
            {
                puntuacion += 10;
                txt.text = "Puntuación: " + puntuacion;
                anim.SetTrigger("Happy");
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                Celebrate();
            }
            // Choque con caca
            if (collision.gameObject.CompareTag("Caca"))
            {
                // Perdemos vidas y refrescamos
                vidas -= 1;
                if(vidas == 0)
                {
                    Lost();
                }
                setLifes();
                anim.SetTrigger("Sad");
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (running)
        {
            if (collision.gameObject.CompareTag("SkeletalGround"))
            {
                if (dmgCD <= 0.0f)
                {
                    vidas -= 1;
                    setLifes();
                    dmgCD = 2.0f;
                }
                anim.SetTrigger("Sad");
            }
        }
        if (collision.gameObject.CompareTag("BoneGround"))
        {
            anim.SetTrigger("Happy");
        }
        
    }
    /**
     * Función para mostrar las vidas en el HUD
     * */
    public void setLifes()
    {
        heart1.enabled = true;
        heart2.enabled = true;
        heart3.enabled = true;
        if(vidas == 6)
        {
            heart1.sprite = fullheart;
            heart2.sprite = fullheart;
            heart3.sprite = fullheart;
            return;
        }
        if(vidas == 5)
        {
            heart1.sprite = halfheart;
            heart2.sprite = fullheart;
            heart3.sprite = fullheart;
            return;
        }
        if (vidas == 4)
        {
            heart1.enabled = false;
            heart2.sprite = fullheart;
            heart3.sprite = fullheart;
            return;
        }
        if (vidas == 3)
        {
            heart1.enabled = false;
            heart2.sprite = halfheart;
            heart3.sprite = fullheart;
            return;
        }
        if (vidas == 2)
        {
            heart1.enabled = false;
            heart2.enabled = false;
            heart3.sprite = fullheart;
            return;
        }
        if (vidas == 1)
        {
            heart1.enabled = false;
            heart2.enabled = false;
            heart3.sprite = halfheart;
            return;
        }
    }
    /**
     * UI al perder
     * */
    public void Lost() {
        running = false;
        HUD.enabled = false;
        EndGame.enabled = true;
        EndGameTxt.text = "Has perdido \n Puntuación: " + puntuacion;

    }
    /**
     * UI al ganar
     * */
    public void Won()
    {
        HUD.enabled = false;
        EndGame.enabled = true;
        EndGameTxt.text = "Has ganado \n Puntuación: " + puntuacion;
        
    }
    /**
     * Hacemos que los perros salten
     * */
    public void Celebrate()
    {
        foreach(GameObject i in GameObject.FindGameObjectsWithTag("CelebrationDog"))
        {
            i.GetComponent<Rigidbody2D>().AddForce(Vector2.up * celebrationForce);
        }
    }
}
