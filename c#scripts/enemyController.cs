using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class enemyController : MonoBehaviour
{

    public Transform playerTransform;
    private Vector3 direction;
    static Animator anim2;
    public int enemyHealth = 100;
    public static enemyController instance;
    public Slider enemyHB;
    public BoxCollider[] c;
    public AudioClip[] audioClip;
    AudioSource audio;
    private Vector3 enemyPosition;

    void Awake(){
        if(instance == null){
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        anim2 = GetComponent<Animator>();
        SetAllBoxColliders(false);
        audio=GetComponent<AudioSource>();
        enemyPosition = transform.position;
    }

    public void playAudio(int clip){
        audio.clip = audioClip[clip];
        audio.Play();
    }

    private void SetAllBoxColliders(bool state){
    //c[0]은 에너미 오른손, c[1]은 오른발
    c[0].enabled = state;
    c[1].enabled = state;
    }
    // Update is called once per frame
    void Update()
    {
        if(anim2.GetCurrentAnimatorStateInfo(0).IsName("fight_idleCopy")){
            direction = playerTransform.position - this.transform.position;
            //상대랑 가까워졌을 때 공회전하거나 상대한테 겹쳐지게 드러눕거나 하는 버그 방지
            direction.y=0;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction),0.3f);
            SetAllBoxColliders(false);    
        }

        Debug.Log(direction.magnitude);
        if(direction.magnitude>13f && gameController.allowMovement == true){
            anim2.SetTrigger("walkFWD");
            SetAllBoxColliders(false);
            audio.Stop();
        } else {
            //idle은 loop조건이 되어있기 때문에 idle 세팅해줄 필요는 없고 walkFWD만 정지시키면 됨
            anim2.ResetTrigger("walkFWD");
        }

        if(direction.magnitude<13f && direction.magnitude>8 &&gameController.allowMovement == true){
            SetAllBoxColliders(true);
            if(!audio.isPlaying && !anim2.GetCurrentAnimatorStateInfo(0).IsName("roundhouse_kick 2")){
                playAudio(1);
                anim2.SetTrigger("kick");
            }

        } else {
            anim2.ResetTrigger("kick");
        }

        if(direction.magnitude<6f &&gameController.allowMovement == true){
            SetAllBoxColliders(true);
            if(!audio.isPlaying && !anim2.GetCurrentAnimatorStateInfo(0).IsName("cross_punch")){
                playAudio(0);
                anim2.SetTrigger("punch");
            }
            

        } else {
            anim2.ResetTrigger("punch");
        }

        if(direction.magnitude > 0f && direction.magnitude<2 && gameController.allowMovement ==true)
        {
            anim2.SetTrigger("walkBack");
            SetAllBoxColliders(false);
            audio.Stop();
        } else {
            anim2.ResetTrigger("walkBack");
        }

    }

    public void enemyReact(){
        enemyHealth = enemyHealth - 10;
        enemyHB.value = enemyHealth;
        if(enemyHealth<10){
            enemyKnockout();
        } else{
            anim2.ResetTrigger("idle");
            anim2.SetTrigger("react");
        }

    }

    public void enemyKnockout(){
        gameController.allowMovement = false;
        enemyHealth= 100;
        anim2.SetTrigger("knockout");
        gameController.instance.scorePlayer();
        gameController.instance.onScreenPoints();
        gameController.instance.rounds();

        if(gameController.playerScore == 2){
            gameController.instance.doReset();
        }else{
            StartCoroutine(resetCharacters());
        }
    }

    IEnumerator resetCharacters(){
        yield return new WaitForSeconds(4);
        enemyHB.value = 100;
        //reset position
        GameObject[] theclone = GameObject.FindGameObjectsWithTag("Enemy");
        Transform t = theclone[0].GetComponent<Transform>();
        t.position = enemyPosition;
        t.position = new Vector3(t.position.x,0.09f,t.position.z);
        gameController.allowMovement = true;
    }
}
