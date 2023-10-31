using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPower : MonoBehaviour
{
    //Este codigo es el funcionamiento de uno de los poderes que existen en el juego. Lanzamiento de la pelota en una direccion concreta.
    [SerializeField] GameObject ball;
    [SerializeField] GameObject arrowInstancie;
    [SerializeField] float speed;

    GameObject arrow;
    bool arrowSpawn;
    PlayerMove player;
    Animator animat;
    [SerializeField] Transform tr;

    void Start()
    {
        player = GetComponent<PlayerMove>();
        animat = GetComponent<Animator>();
        arrowSpawn = false;
    }

    public void Launch(int playernum)
    {
        ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        //A esta funcion se le pasa un numero para identificar si es el jugador uno o el dos.
        //Así se coloca la bola delante del jugador que ha activado el poder y se lanza la bola en la direeción adecuada.
        if (playernum == 1)
        {
            ball.transform.position = new Vector2(tr.position.x + 1, tr.position.y); //Se coloca la bola delante del jugador.
            if (!arrowSpawn)//Para generar una sola flecha de dirección.
            {
                arrowSpawn = true;
                arrow = Instantiate(arrowInstancie, new Vector3 (tr.position.x +1 ,tr.position.y,tr.position.z), Quaternion.identity);
                //Se genera la flecha delante del jugador sin cambiar su rotación.
                //La flecha rota controlada por el animator.
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 direction = Quaternion.Euler(0, 0, (arrow.transform.rotation.z) * Mathf.Rad2Deg) * Vector3.right;
                //Se capta la direccion en la que apunta la flecha en el momento en el que se presiona espacio.
                Vector2 dir = transform.TransformDirection(direction.normalized);//se normaliza el vector para no alterar la velocidad de lanzamiento
                animat.SetBool("launched", true);
                ball.GetComponent<Rigidbody2D>().AddForce(dir * speed, ForceMode2D.Impulse); 
                //Se coge esta dirección y se lanza la bola con una velocidad determinada (Speed)
                player.powerImage.gameObject.GetComponent<UIPower>().SetImage(0);
                player.ArrowPowerActive = false;
                player.power = 0;
                player.Powers[0] = false;
                Destroy(arrow);
                arrowSpawn = false;
                //Se borra la flecha y se desactiva el poder.
            }

        }
        if (playernum == 2)
        {
            // esto es exactamente lo mismo que lo anterior pero en direccion opuesta.
            ball.transform.position = new Vector2(tr.position.x - 1, tr.position.y);
            if (!arrowSpawn)
            {
                arrowSpawn = true;
                arrow = Instantiate(arrowInstancie, new Vector3(tr.position.x - 1, tr.position.y, tr.position.z), Quaternion.identity);
                
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 direction = Quaternion.Euler(0, 0, (arrow.transform.rotation.z) * Mathf.Rad2Deg) * Vector3.right;
                Vector2 dir = transform.TransformDirection(direction.normalized);

                animat.SetBool("launched", true);
                ball.GetComponent<Rigidbody2D>().AddForce(-dir * speed, ForceMode2D.Impulse);
                player.powerImage.gameObject.GetComponent<UIPower>().SetImage(0);
                player.ArrowPowerActive = false;
                player.power = 0;
                player.Powers[0] = false;
                Destroy(arrow);
                arrowSpawn = false;
            }
        }
    }
}
