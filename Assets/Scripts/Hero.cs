using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {

	public static float health = 40.0f;
	bool fired = false;
	bool firing = false;
	float reloadTime = 1.5f;

	Vector2 startPos;
	Vector2 endPos;


	Animator bowAnim;
	Transform bowTran;
	Transform arrowTran;
	Transform arrowSpawn;
	HeroArm hand;

	float distance = 0.0f;
	float angle = 0.0f;
	Vector2 direction;

	Rigidbody2D arrowInstance;
	public Rigidbody2D arrowType;

	float playTime = 0.0f;

	static Color damageColor;

	static GUITexture damageTexture;
	public GUITexture textureType;


	// Use this for initialization
	void Start () {

		arrowTran = transform.FindChild("heroarrow");
		bowTran = transform.FindChild("HeroArm");
		arrowSpawn = bowTran.FindChild("heroarm").FindChild("arrowSpawn");
		bowAnim = bowTran.GetComponent<Animator>();
		hand = bowTran.GetComponent ("HeroArm") as HeroArm;
		hand.HideHand();

		damageTexture = GameObject.Instantiate (textureType, new Vector3 (0, 0, 0), Quaternion.Euler (new Vector3 (0, 0, 0))) as GUITexture;
		damageColor = new Color (1,1,1,0);
		damageTexture.color = damageColor;
	}
	void OnLevelWasLoaded(int level) {
		health = 40.0f;
		playTime = 0.0f;
		
	}
	
	// Update is called once per frame
	void Update () {

		playTime += Time.deltaTime;

		if (fired) {
			reloadTime -= Time.deltaTime;

			if (reloadTime <= 0.0f) {
				fired = false;
				reloadTime = 1.5f;
				arrowTran.renderer.enabled = true;
			}
		}   

		if (Input.GetMouseButtonUp (0)) {
			OnMouseUpF();
		}
		else if (Input.GetMouseButton (0)) {
			OnMouseDownF();
		}


	}

	void OnMouseDownF() {
		if (!fired && !firing) {

				arrowInstance = Instantiate (arrowType, arrowSpawn.position, Quaternion.Euler (new Vector3 (0, 0, 0))) as Rigidbody2D;
				arrowTran.renderer.enabled = false;
				bowAnim.SetTrigger ("pull");
				firing = true;
				startPos = Input.mousePosition;
				hand.ShowHand ();


		} else {
			OnMouseDragF();
		}
	}

	void OnMouseUpF () {
		if (firing) {
			bowAnim.SetTrigger("release");
			firing = false;
			fired = true;


			GameObject[] myArrows = GameObject.FindGameObjectsWithTag("Arrow");

			foreach(GameObject arrow in myArrows)
			{
				Arrow ar = arrow.GetComponent("Arrow")as Arrow;

				if(ar != null && !ar.isFlying)
				{
					distance = Mathf.Max(300.0f, distance);
					distance = Mathf.Min(800.0f, distance);
					ar.Launch(direction, distance);
				}
			}

		}

	}

	void OnMouseDragF () {
		if (firing) {
			endPos = Input.mousePosition;

			direction = (startPos - endPos);
			
			angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			distance = Vector2.Distance(endPos, startPos);

			//Debug.Log(angle.ToString());
			//Debug.Log(direction.ToString());
			if(arrowInstance != null)
			{
				arrowInstance.transform.position = arrowSpawn.position;
				arrowInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}

			bowTran.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}

	}



	public static void GetHit(float damage){
		health -= damage;

		damageColor = new Color (1,1,1,(40.0f - health) / 100f);
		damageTexture.color = damageColor;

		if (health <= 0.0f) {
			GameOver();
		}
	}

	static void GameOver(){
		Application.LoadLevel("GameOver");
	}
}
