using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    //En este script encontramos el movimiento y los poderes de los dos jugadores

    [SerializeField]
    int speed;
    [SerializeField]
    GameObject ball;
    [SerializeField]
    public GameObject powerImage;
    [SerializeField] GameObject clone;

    Vector3 player;

    Animator animat;

    ArrowPower ArrowPower;
    WallPower WallPower;
    InvisiblePower InvisiblePower;  //Cada poder esta en su propio script y se le llama desde aqui
    FastshotPower FastShotPower;
    public bool move;
    public float power;
    public bool [] Powers = new bool [5];//Array para guardar los poderes y ver si estan activos o no
    [SerializeField] float MaxPower;
    [SerializeField] Slider barr;
    [SerializeField] GameObject powerCollider;
    public bool ArrowPowerActive = false;
    public bool FastShotPowerActive = false;
    float time;
    bool hited;
    void Start()
    {
        Powers[0] = false;
        Powers[1] = false;
        Powers[2] = false;
        Powers[3] = false;
        Powers[4] = false;
        move = true;
        power = 0;
        ArrowPower = GetComponent<ArrowPower>();
        WallPower = GetComponent<WallPower>();
        InvisiblePower = GetComponent<InvisiblePower>();
        FastShotPower = GetComponent<FastshotPower>();
        animat = GetComponent<Animator>();
        barr.maxValue = MaxPower;
    }
    void Update()
    {
        //Para no hacer un script para cada jugador he dividido el update en dos partes dependiendo de si es el Player1 o Player2
        barr.value = power;
        if (gameObject.CompareTag("Player1") && move)
        {
            Move("Vertical"); // Funcion de movimiento de los Player que se encuentra mas abajo en el script
            if (Input.GetKeyDown(KeyCode.Q) && power > MaxPower)
            {
                // la activación de cada uno de los poderes
                //depende del poder se activa llamando directamente a la funcion del Script determinado desde aqui o se activa con un booleano
                //estas ultimas son las que desativan el movimiento del player
                if(Powers[0] == true && powerCollider.GetComponent<PowerCollider>().trigger) //Para los poderes en los que necesitas tener la pelota cerca hay un collider que detecta esto
                {
                    animat.SetBool("movup", false);//se desactivan las animaciones 
                    animat.SetBool("movedown", false);
                    ArrowPowerActive = true;
                }
                if(Powers[1] == true)
                {
                    WallPower.launch(1);
                }
                if (Powers[2] == true)
                {
                    InvisiblePower.ActivePower();
                }
                if (Powers[3] == true && powerCollider.GetComponent<PowerCollider>().trigger)
                {
                    animat.SetBool("movup", false);
                    animat.SetBool("movedown", false);
                    FastShotPowerActive = true;
                }
                if (Powers[4] == true)
                {
                    Instantiate(clone, transform);
                    power = 0;
                    Powers[4] = false;
                    powerImage.gameObject.GetComponent<UIPower>().SetImage(0);
                }
            }
        }
        if (gameObject.CompareTag("Player2") && move)
        {
            Move("Vertical2");
            if (Input.GetKeyDown(KeyCode.LeftArrow) && power > MaxPower)
            {
                if (Powers[0] == true && powerCollider.GetComponent<PowerCollider>().trigger)
                {
                    animat.SetBool("movup", false);
                    animat.SetBool("movedown", false);
                    ArrowPowerActive = true;
                }
                if (Powers[1] == true)
                {
                    WallPower.launch(2);
                }
                if (Powers[2] == true)
                {
                    InvisiblePower.ActivePower();
                }
                if (Powers[3] == true && powerCollider.GetComponent<PowerCollider>().trigger)
                {
                    animat.SetBool("movup", false);
                    animat.SetBool("movedown", false);
                    FastShotPowerActive = true;
                }
                if (Powers[4] == true)
                {
                    power = 0;
                    Powers[4] = false;
                    powerImage.gameObject.GetComponent<UIPower>().SetImage(0);
                    Instantiate(clone, transform);
                }
            }
        }
        if (hited == true)//se pone un delay entre hits con la pelota hacia los players para que la barra de poder no se cargue instantaneamente
        {
            time += Time.deltaTime;
            if (time >= 1)
            {
                time = 0;
                hited = false;
            }
        }
        if (ArrowPowerActive) // En el poder de lanzamiento de flecha y del disparo rapido se desactiva el movimiento y se llama a la funcion del script determinado
        { 
            move = false;
            if (gameObject.CompareTag("Player1"))
                ArrowPower.Launch(1);

            if (gameObject.CompareTag("Player2"))
                ArrowPower.Launch(2);
        }else
        if(FastShotPowerActive)
        {
            move = false;
            if (gameObject.CompareTag("Player1"))
                FastShotPower.launch(1);

            if (gameObject.CompareTag("Player2"))
                FastShotPower.launch(2);
        }
        else
        {
            move = true;
        }
    }
    void Move(string verticalAxis)
    {
        player = new Vector3(0, Input.GetAxis(verticalAxis) * Time.deltaTime, 0); // funcion para mover la y de los objetos player
        transform.position += player * speed;

        if ((player.y == 0))// si la y es 0 se queda en idle
        {
            animat.SetBool("movup", false);
            animat.SetBool("movedown", false);
        }
        else
        {
            if (player.y > 0)// si la y es positiva (te estas moviendo hacia arriba)
            {
                animat.SetBool("movup", true);
                animat.SetBool("movedown", false); 
            }
            if (player.y < 0)// si la y es positiva (te estas moviendo hacia abajo)
            {
                animat.SetBool("movup", false);
                animat.SetBool("movedown", true);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bola"))
        {
            animat.SetTrigger("hit");
            if (power < MaxPower && !hited)
            {
                power ++; //cada vez que golpea la pelota al player se va sumando poder
                hited = true;
            }
            if(power == MaxPower)
            {   //cuando se llega al poder maximo se seleciona un poder al azar y este se pone en true para poder activarlo
                int powernum = Random.Range(0,Powers.Length);
                Powers[powernum] = true;
                powerImage.gameObject.GetComponent<UIPower>().SetImage(powernum+1);
                power++; //se suma para que deje de ser igual y hasta que no vuelva a 0 no se pueda cambiar de poder
            }
        }
    }
}
