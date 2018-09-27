
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Linq;
	using Newtonsoft.Json;
	using KMEdgework;

	public class manometers : MonoBehaviour {


		public KMAudio Audio;
		public KMBombModule Module;
		public KMBombInfo Info;
		//public KMModSettings modSettings;
		public KMSelectable Tp,Tm,BLp,BLm,BRp,BRm,abort,sP,sM,screen;
		public TextMesh T, BL,BR, screenT;
		public GameObject valve, piston;
	public MeshRenderer pScreen, minus, plus;
	private KMAudio.KMAudioRef timerSound;

		private static int _moduleIdCounter = 1;
		private int _moduleId = 0;
	private int timer = 30;
	private string topC, blC, brC;
	private string minC, plusC, screenC;

		private int maxPT, maxPBL, maxPBR, presT, presBL, presBR;
		private int  gP, pressure = 9;
		private int colT, colBL, colBR;
		private const int BLUE = 0, RED = 2, GREEN = 1;
	private bool _isSolved = false, _lightsOn = false, _valve = false, submit = false;
	private bool greenS = false, orange = false, turn = false;
	private int min, plu, scr;
	private int reaP;
	private int[,] combinations = new int[,] 
	{
		{RED,BLUE,RED},
		{GREEN,GREEN,GREEN},
		{BLUE,RED,GREEN},
		{GREEN,BLUE,RED},

		{BLUE,BLUE,RED},
		{RED,GREEN,BLUE}, //
		{GREEN,GREEN,RED},
		{GREEN,BLUE,GREEN},

		{GREEN,RED,BLUE},
		{RED,RED,GREEN},
		{BLUE,BLUE,BLUE},
		{BLUE,GREEN,RED},

		{RED,BLUE,GREEN},
		{BLUE,RED,RED},
		{GREEN,GREEN,BLUE},
		{RED,GREEN,GREEN},

		{RED,RED,RED},
		{BLUE,BLUE,GREEN},
		{GREEN,RED,GREEN},
		{BLUE,GREEN,BLUE},													
			
		{BLUE,GREEN,GREEN},
		{RED,GREEN,RED},
		{GREEN,BLUE,BLUE}, 	
		{RED,BLUE,BLUE},

		{RED,RED,BLUE},				
		{GREEN,RED,RED},									
		{BLUE,RED,BLUE}};

	private int[,] pressureList = new int[,] 
	{
		{3,4,4},
		{10,9,9},
		{2,10,5},
		{6,10,7},

		{2,6,3},
		{8,10,2}, //
		{10,10,10},
		{7,9,9},

		{4,2,2},
		{8,6,7},
		{6,5,1},
		{4,6,2},

		{9,10,8},
		{3,4,2},
		{8,6,9},
		{7,2,6},

		{5,3,2},
		{6,7,10},
		{8,10,10},
		{6,10,4},

		{7,9,5},
		{5,6,9},
		{8,6,2}, 
		{1,5,3},
	
		{9,8,7},
		{8,8,9},
		{2,4,5},};
	
	private int[] actualCombination = new int[3] {0,0,0};

		//private int stageAmt, stageCur = 1, ans, inputAns = 0, threshold = 10;

		void Start ()
	{
			_moduleId = _moduleIdCounter++;
			Module.OnActivate += Activate;
		}

		private void Awake()
	{
			abort.OnInteract += delegate() {
				abortHandle ();
				return false;
			};
			Tp.OnInteract += delegate {
				plusHandle ("top");
				return false;
			};
			BLp.OnInteract += delegate {
				plusHandle ("bl");
				return false;
			};
			BRp.OnInteract += delegate() {
				plusHandle ("br");
				return false;
			};
			Tm.OnInteract += delegate {
				minusHandle ("top");
				return false;
			};
			BLm.OnInteract += delegate {
				minusHandle ("bl");
				return false;
			};
			BRm.OnInteract += delegate() {
				minusHandle ("br");
				return false;
			};

			sP.OnInteract += delegate() {
				screenHandle ("+");
				return false;
			};
			sM.OnInteract += delegate() {
				screenHandle ("-");
				return false;
			};
			screen.OnInteract += delegate {
				screenHandle ("s");
				return false;
			};
		
	}

		

		void Activate()
	{
			Init();
			_lightsOn = true;
		}

		void Init()
		{
		StartCoroutine ("blink");
		reachedPressure ();
		}

		void abortHandle(){
		if (!_isSolved&&_lightsOn) {
			if (submit&&!turn) {
				StartCoroutine ("valveR");
			}
		}
	}
	IEnumerator blink(){
		
		while (pressure!=10) {
			screenT.text = "--";
			for (float i = 0; i < .2; i += 0.01f) {
				if (pressure != 10) {
					yield return new WaitForSecondsRealtime (0.01f);
				}
			}	if (pressure != 10) {

				screenT.text = "";
				for (float i = 0; i < .2; i += 0.01f) {

					if (pressure != 10) {
						yield return new WaitForSecondsRealtime (0.01f);
					}
				}
			}
		}
	}
	void reachedPressure(){
		int color = Random.Range (0, 5);
		switch (color) {
		case 0:
			pScreen.material.color = Color.blue;  //IM BLUE DABEDI DABOODA 
			greenS = true;
			screenC = "blue";
			scr = 5;
			break;
		case 1:
			pScreen.material.color = new Color32 (214, 92, 6, 255);
			orange = true;
			screenC = "orange";
			scr = 7;
			break;
		case 2:
			pScreen.material.color = Color.black;
			scr = 8;
			screenC = "black";
			break;
		case 3:
			pScreen.material.color = Color.yellow;
			scr = 9;
			screenC = "yellow";
			break;
		case 4:
			pScreen.material.color = Color.magenta;
			scr = 6;
			screenC = "magenta";
			break;
		}

		color = Random.Range (0, 3);

		switch (color) {
		case 0:
			min = 2;
			minus.material.color = Color.blue;  //IM BLUE DABEDI DABOODA 
			minC = "blue";
			break;
		case 1:
			min = 3;
			minus.material.color = new Color32 (214, 92, 6, 255);
			minC = "orange";

			break;
		case 2:
			min = 4;
			minus.material.color = Color.yellow;
			minC = "yellow";
			break;
		}

		color = Random.Range (0, 3);

		switch (color) {
		case 0:
			plus.material.color = Color.blue;  //IM BLUE DABEDI DABOODA 
			plu = 1;	plusC = "blue";
			break;
		case 1:
			plus.material.color = new Color32 (214, 92, 6, 255);
			plusC = "orange";
			plu = 2;
			break;
		case 2:
			plus.material.color = Color.yellow;
			plu = 1;plusC = "yellow";
			break;
		}
		//Debug.LogFormat ("{0}, {1}, {2}", scr, min, plu);
		gP = (min * scr) / plu;

		while (gP > 35 || gP <11) {
			if (gP < 11) {
				gP += 4;
			}else if (gP>35)
			{gP -= 6;}
		}
		Debug.LogFormat ("[Manometers #{0}] Screen is {2}, minus button is {3} and plus button is {4}, therefore the pressure to reach is {1}", _moduleId, gP, screenC, minC, plusC);
	}


	void generatePressure (int mano, int pres, TextMesh manoText)
	{
	//	Debug.LogFormat ("PRESSURIZATION");
		pres = 0;
		switch (mano) {
		case BLUE:
			{
				manoText.color = new Color32 (28, 172, 236, 255);	
				break;
			}
		case GREEN:
			{
				manoText.color = new Color32 (0, 255, 65, 255);	
				break;}
		case RED:
			{
				manoText.color = new Color32 (226, 43, 11, 255);
					break;
			}


		}/*	if (manoText == T) {Debug.LogFormat ("T");
			maxPT = pres;
			top = false;
		} else if (manoText == BL) {Debug.LogFormat ("BL");
			maxPBL = pres;bl = false;
		} else if (manoText == BR) {Debug.LogFormat ("BR");
			maxPBR = pres;br = false;
		}*/
	}


		void startMano()
	{
		int comb = 28;
	//	Debug.LogFormat("{0}",_moduleIdCounter);

		presT = 0;
		presBL = 0;
		presBR = 0;
		T.text = (string)presT.ToString();
		BL.text = (string)presBL.ToString();
		BR.text = (string)presBR.ToString ();
		colT = Random.Range (0, 3);
			colBL = Random.Range (0, 3);
			colBR = Random.Range (0, 3);
		int greenC=0;
		int redC=0;
		actualCombination [0] = colBL;
		actualCombination [1] = colT;
		actualCombination [2] = colBR;
		generatePressure (colT, presT, T);
		generatePressure (colBL, presBL, BL);
		generatePressure (colBR, presBR, BR);
		for (int u = 0; u < 27; u++) {
			int botL=0, top=0, botR=0;
			//Debug.LogFormat ("{0}", u);
			for (int o = 0; o < 3; o++) {

				switch (o) {
				case 0:
					botL = combinations [u,o];
					break;
				case 1:
					top = combinations [u,o];
					break;
				case 2: 
					botR = combinations [u,o];
					break;
				}
			}
			if (actualCombination [0] == botL && actualCombination [1] == top && actualCombination [2] == botR) {
				comb = u;
				break;
			}

		}

		switch (comb) {
		case 10:
			if (Info.GetBatteryCount () >= 2) {
				comb += 2;
			} else if (Info.IsPortPresent ("RJ45")) {
				comb -= 2;
			} else if (System.DateTime.Now.Month == 8) {
				comb += 3;
			} else {
				comb -= 1;
			}
			break;
		case 17:
			if (Info.GetPortCount () >= 3) {
				comb += 5;
			} else if (Info.IsIndicatorOff ("FRQ")) {
				comb -= 4;
			} else if (orange) {
				comb += 1;
			} else {
				comb += 4;
			}
			break;
		case 22:
			if (Info.IsPortPresent ("RJ45")) {
				comb += 2;
			} else if (Info.GetBatteryCount (Battery.D) == 1) {						//WARNING : NEED TO KNOW THE D BATERIES COUNT
				comb -= 2;
			} else if (Info.GetBatteryCount () > 2) {
				comb += 3;
			} else {
				comb += 5;
			}
			break;
		case 4:
			if (System.DateTime.Now.DayOfWeek.Equals ("Monday")) {
				comb += 3;
			} else if (Info.GetBatteryHolderCount () >= 2) {
				comb -= 1;
			} else if (System.DateTime.Now.DayOfWeek.Equals ("Friday") && System.DateTime.Now.Date.Equals (13)) {
				comb += 10;
			} else {
				comb += 2;
			}
			break;
		case 23:
			if (Info.GetOnIndicators ().Contains ("SIG")) {
				comb += 3;
			} else if (Info.GetOffIndicators ().Contains ("NSA")) {
				comb += 2;
			} else if (Info.GetIndicators ().Count () > 2) {
				comb += 5;
			} else {
				comb -= 2;
			}
			break;
		case 12:
			if (Info.IsPortPresent ("DVI")) {
				comb -= 1;
			} else if (Info.IsPortPresent ("PS2")) {
				comb -= 3;
			} else if (Info.IsPortPresent ("Parallel")) {
				comb += 4;
			} else {
				comb += 2;
			}
			break;
		case 3:
			if (System.DateTime.Now.Month == 5) {
				comb += 3;
			} else if (Info.IsIndicatorOff ("FRK")) {
				comb -= 8;
			} else if (Info.GetBatteryCount (Battery.AA) + Info.GetBatteryCount (Battery.AAx3) + Info.GetBatteryCount (Battery.AAx4) > Info.GetBatteryCount (Battery.D)) { 				//WARNING : NEED TO KNOW AA>D
				comb += 2;
			} else {
				comb -= 2;
			}
			break;
		case 0:
			if (Info.GetBatteryCount (Battery.AA) + Info.GetBatteryCount (Battery.AAx3) + Info.GetBatteryCount (Battery.AAx4) <= Info.GetBatteryCount (Battery.D)) {				//WARNING : NEED TO KNOW AA<=D
				comb += 2;
			} else if (Info.IsIndicatorOn ("TRN")) {
				comb += 1;
			} else if (System.DateTime.Now.Month == 2) {
				comb += 4; 
			} else {
				comb += 2;
			}
			break;
		case 7:
			if (Info.IsIndicatorOn ("MSA")) {
				comb += 2;
			} else if (Info.IsPortPresent ("StereoRCA")) {
				comb -= 2;
			} else if (Info.GetBatteryCount () == 0) {
				comb += 3;
			} else {
				comb -= 3;
			}
			break;
		case 1:
			if (Info.GetSerialNumberLetters ().ToString ().Contains ("A") || Info.GetSerialNumberLetters ().ToString ().Contains ("E") || Info.GetSerialNumberLetters ().ToString ().Contains ("I") || Info.GetSerialNumberLetters ().ToString ().Contains ("O") || Info.GetSerialNumberLetters ().ToString ().Contains ("U") || Info.GetSerialNumberLetters ().ToString ().Contains ("Y")) {
				comb += 1;
			} else if (Info.IsIndicatorOff ("CLR")) {
				comb -= 5;
			} else if (Info.IsIndicatorOn ("CAR")) {
				comb += 2;
			} else {
				comb += 5;
			}
			break;
		case 14:
			if (System.DateTime.Now.DayOfWeek.Equals ("THURSDAY")) {
				comb += 5;
			} else if (Info.GetPortPlateCount () > 2) {
				comb -= 1;
			}else if(Info.GetPortCount(Port.RJ45)>=2){				//MAYDAY MAYDAY NEED TO KNOW HOW MANY RJ45 THERE IS
				comb-=3;
			
			} else {
				comb -= 3;
			}
			break;
		case 20:
			
			if (Info.GetModuleNames ().Contains ("Dr. Doctor")) {
				comb += 2;
			} else if (Info.IsIndicatorOn ("IND")) {
				comb -= 8;
			} else if (System.DateTime.Now.Month%2==0) {
				comb += 1;
			} else {
				comb -= 4;
			}
			break;
		case 6:
			if (Info.GetBatteryCount (Battery.D)< Info.GetBatteryCount (Battery.AA) + Info.GetBatteryCount (Battery.AAx3) + Info.GetBatteryCount (Battery.AAx4)) {			//WARNING NEED TO KNOW D<AA
				comb += 2;
			} else if (Info.GetIndicators ().Count () > 2) {
				comb -= 1;
			} else if (Info.GetSerialNumberNumbers ().Last () % 2 != 0) {
				comb += 1;
			} else {
				comb += 4;
			}
			break;
		case 15:
			if (System.DateTime.Now.Hour > 17) {
				comb += 2;
			} else if (Info.GetBatteryCount (Battery.AA) + Info.GetBatteryCount (Battery.AAx3) + Info.GetBatteryCount (Battery.AAx4) == Info.GetIndicators ().Count ()) {			//WARNING NEED TO KNOW AA==IND
				comb -= 2;
			} else if (Info.IsIndicatorOn ("FRQ")) {
				comb += 3;
			} else {
				comb += 5;
			}
			break;
		case 5:
			if (Info.IsPortPresent ("DVI")) {
				comb += 2;
			} else if (Info.IsIndicatorOn ("NSA")) {
				comb -= 2;
			} else if (System.DateTime.Now.DayOfWeek.ToString ().Equals ("TUESDAY")) {
				comb += 3;
			} else {
				comb += 3;
			}
			break;
		case 11:
			if (System.DateTime.Now.Month == 3) {
				comb -= 1;
			} else if (Info.IsIndicatorOff ("CLR")) {
				comb += 3;
			} else if (Info.IsPortPresent ("Serial")) {
				comb += 2;
			} else {
				comb -= 3;
			}
			break;
		case 19:
			if (greenS) {
				comb += 1;
			} else if (System.DateTime.Now.Month == 1) {
				comb += 6;
			} else if (Info.GetStrikes () == 2) {
				comb -= 4;
			} else {
				comb += 2;
			}
			break;
		case 21:
			if (Info.GetStrikes () > Info.GetIndicators ().Count ()) {
				comb += 2;
			} else if (Info.IsIndicatorOn ("CLR")) {
				comb -= 2;
			} else if (Info.GetBatteryCount (Battery.AA) + Info.GetBatteryCount (Battery.AAx3) + Info.GetBatteryCount (Battery.AAx4) == Info.GetStrikes ()) {						//WARNING NEED TO KNOW AA==STR
				comb += 3;
			} else {
				comb += 2;
			}
			break;
		case 16:
			if (System.DateTime.Now.Hour > 20) {
				comb += 9;
			} else if (Info.IsIndicatorOn ("SND")) {
				comb -= 15;
			} else if (Info.IsIndicatorOff ("CAR")) {
				comb += 3;
			} else {
				comb += 2;
			}
			break;
		case 24:
			if (Info.IsIndicatorOn ("SIG")) {
				comb += 5;
			} else if (Info.IsIndicatorOff ("SIG")) {
				comb -= 1;
			}else if(Info.GetBatteryCount (Battery.D)==Info.GetPortCount(Port.Serial)){							//MAYDAY MAYDAY      -----------------     WARNING NEED TO KNOW D==SERIAL
				comb-=6;		
			} else {
				comb += 4;
			}
				
			break;
		case 13:
			if (Info.IsIndicatorOn ("BOB")) {
				comb += 21;
			} else if (System.DateTime.Now.Month == 7) {
				comb -= 5;
			} else if (Info.IsPortPresent ("Parallel")) {
				comb += 1;
			} else {
				comb -= 3;
			}
			break;
		case 9:
			if (Info.IsIndicatorOff ("CLR")) {
				comb += 1;
			} else if (Info.IsPortPresent ("StereoRCA")) {
				comb -= 9;
			} else if (System.DateTime.Now.Month == 10) {
				comb += 5;
			} else {
				comb += 5;
			}
					
			break;
		case 25:
			if (Info.GetTime () < (5 * 60)) {
				comb -= 3;
			} else if (Info.IsIndicatorOff ("NSA")) {
				comb -= 1;
			} else if (System.DateTime.Now.DayOfWeek.Equals ("SUNDAY")) {
				comb += 4;
			} else {
				comb += 4;
			}
			break;
		case 2:
			if (System.DateTime.Now.DayOfWeek.Equals ("WEDNESDAY")) {
				comb += 3;
			} else if (Info.GetModuleNames ().Contains ("LEGO")) {
				comb -= 2;
			} else if (Info.GetBatteryCount (Battery.AA) + Info.GetBatteryCount (Battery.AAx3) + Info.GetBatteryCount (Battery.AAx4) > 2) {												//WARNING NEED TO KNOW AA>2
				comb += 2;
			} else {
				comb -= 5;
			}
			break;
		case 8:
			if (Info.GetSerialNumberLetters ().Contains ('Q')) {
				comb += 6;
			} else if (Info.IsPortPresent ("Serial")) {
				comb -= 1;
			} else if (Info.GetBatteryCount (Battery.D) == Info.GetBatteryCount (Battery.AA) + Info.GetBatteryCount (Battery.AAx3) + Info.GetBatteryCount (Battery.AAx4)) {						//WANRING NEED TO KNOW D==AA
				comb -= 2;
			} else {
				comb -= 5;
			}
			break;
		case 26:
			if (Info.IsIndicatorOn ("TRN")) {
				comb += 1;
			} else if (Info.IsPortPresent ("PS2")) {
				comb -= 2;
			} else if (System.DateTime.Now.Date.ToString ().Equals ("12/25")) {
				comb += 5;
			} else {
				comb += 1;
			}
			break;
		case 18:
			if (Info.IsIndicatorOff ("BOB")) {
				comb += 3;
			} else if (System.DateTime.Now.Month == 4) {
				comb += 2;
			} else if (System.DateTime.Now.DayOfWeek.Equals ("SATURDAY")) {
				comb += 1;
			
			} else {
				comb -= 2;
			}
			break;
		
		}if (comb >= 27) {
			comb -= 27;
		} else if (comb < 0) {
			comb += 27;
		}
			maxPT = pressureList [comb, 1];
		maxPBL = pressureList [comb, 0];
		maxPBR = pressureList [comb, 2];

		switch (colT) {
		case GREEN:
			topC = "green";
			break;
		case RED:
			topC = "red";
			break;
		case BLUE:
			topC = "blue";
			break;
		}
		switch (colBL) {
		case GREEN:
			blC = "green";
			break;
		case RED:
			blC = "red";
			break;
		case BLUE:
			blC = "blue";
			break;
		}switch (colBR) {
		case GREEN:
			brC = "green";
			break;
		case RED:
			brC = "red";
			break;
		case BLUE:
			brC = "blue";
			break;
		}

		Debug.LogFormat ("[Manometers #{0}] The top manometer is {1}, the bottom left one is {2} and the bottom right one is {3}.", _moduleId, topC, blC, brC);
		if (colT == GREEN) {
			greenC++;
		}if (colBL == GREEN) {
			greenC++;
		}if (colBR == GREEN) {
			greenC++;
		}	if (colT == RED) {
			redC++;
		}if (colBL == RED) {
			redC++;
		}if (colBR == RED) {
			redC++;
		}



		if (maxPT+ maxPBL + maxPBR >= gP) {
			_valve = false;
		} else {
			_valve = true;
		}
		if(_valve){
		Debug.LogFormat("[Manometers #{0}] : Max Pressures are : {1} (TOP), {2} (BL), {3} (BR). Pressure to reach is {4}. The valve must be used.", _moduleId, maxPT, maxPBL, maxPBR, gP);
		}else{
			Debug.LogFormat("[Manometers #{0}] : Max Pressures are : {1} (TOP), {2} (BL), {3} (BR). Pressure to reach is {4}. The valve must not be used.", _moduleId, maxPT, maxPBL, maxPBR, gP);
		}
			}
		
					
			
		IEnumerator valveR(){
		
		if (timerSound!=null) {
	//		Debug.LogFormat ("CUT SOUND");
			timerSound.StopSound ();
			timerSound = null;
		}
		Audio.PlaySoundAtTransform("steam", valve.transform);

		turn = true;
		float o = 1f;
			for(int i = 0; i<200;i++){
				valve.gameObject.transform.Rotate(new Vector3 (0,0,o));
				yield return new WaitForSecondsRealtime (0.0001f);
			o += 0.05f;
		} 

		if (_valve)
			//presT==maxPT&&presBL==maxPBL&&presBR==maxPBR && 
			 {
			
			if (presT == maxPT && presBL == maxPBL && presBR == maxPBR) {
				Debug.LogFormat ("[Manometers #{0}] Module solved.", _moduleId);
				_isSolved = true;

			this.HandlePass();
			} else {
				Debug.LogFormat ("[Manometers #{0}] Manometers aren't to their maximum pressure (TOP={1}, BL={2}, BR={3}), that's necessary for the valve to work correctly. 1 Strike and reset !",_moduleId, presT, presBL, presBR);
				this.HandleStrike ();
			}

		} else {
			Debug.LogFormat ("[Manometers #{0}] Valve didn't have to be use. 1 Strike and reset !", _moduleId);
			this.HandleStrike ();
				
		}
		for (int i = 0; i < 200; i++) {
			valve.gameObject.transform.Rotate (new Vector3 (0, 0, o));
			yield return new WaitForSecondsRealtime (0.0001f);
			o -= 0.05f;
		}
		turn = false;

	}

	IEnumerator pressureCount(){
		timerSound = Audio.PlaySoundAtTransformWithRef("countdown", transform);

		for (int u = 0; u < timer; u++) {

			yield return new WaitForSecondsRealtime (1);
		}
		float p = -2.82f;
		for (int u = 0; u < 14; u++) {
			p += 0.1f;
			piston.transform.localPosition = new Vector3 (0, p, 0);
			yield return new WaitForSecondsRealtime (0.001f);
		}
		for (int u = 0; u < 14; u++) {
			p -= 0.1f;
			piston.transform.localPosition = new Vector3 (0, p, 0);
			yield return new WaitForSecondsRealtime (0.001f);
		}
		if (!_isSolved&&!turn) {
			Debug.LogFormat ("[Manometers #{0}] Time ran out ! Strike and reset !", _moduleId);
			this.HandleStrike ();
		}

	}

	void plusHandle(string manometer){

		if (!_isSolved&&!turn) {
			if (submit) {
				Audio.PlaySoundAtTransform ("bip", Module.transform);
				switch (manometer) {
				case "top":
					presT++;
					T.text = (string)presT.ToString ();
					if (presT + presBL + presBR >= gP) {
						Debug.LogFormat ("[Manometers #{0}] Module solved.", _moduleId);
						_isSolved = true;
				
						this.HandlePass();
					}
					if (presT > maxPT) {
						Debug.LogFormat ("[Manometers #{0}] Maximum pressure exceded on the top manometer ! 1 Strike and reset !",_moduleId);
						this.HandleStrike ();
				
					}
					break;
				case "bl":
					presBL++;
					BL.text = (string)presBL.ToString ();
					if (presT + presBL + presBR >= gP) {
						Debug.LogFormat ("[Manometers #{0}] Module solved.", _moduleId);
							_isSolved = true;
				
						this.HandlePass();  
					}
					if (presBL > maxPBL) {
						Debug.LogFormat ("[Manometers #{0}] Maximum pressure exceded on the bottom left manometer ! 1 Strike and reset !",_moduleId);
									this.HandleStrike ();
					
					}
					break;
				case "br":
					presBR++;
					BR.text = (string)presBR.ToString ();
					if (presT + presBL + presBR >= gP) {
						Debug.LogFormat ("[Manometers #{0}] Module solved.", _moduleId);
							_isSolved = true;
				
						this.HandlePass();
					}
					if (presBR > maxPBR) {
						Debug.LogFormat ("[Manometers #{0}] Maximum pressure exceded on the bottom right manometer ! 1 Strike and reset !",_moduleId);
								this.HandleStrike ();
				
					}
					break;
				}
			}
		}
	}

	void minusHandle (string manometer){
		if (!_isSolved&&!turn) {

			if (submit) {
				Audio.PlaySoundAtTransform ("bip", Module.transform);
				switch (manometer) {
				case "top":
					if(!((presT-1)<0)){
					presT--;
					T.text = (string)presT.ToString ();
					}
					break;
				case "bl":
					if (!((presBL - 1) < 0)) {
						presBL--;
						BL.text = (string)presBL.ToString ();
					}
					break;
				case "br":
					if (!((presBR - 1) < 0)) {
						presBR--;
						BR.text = (string)presBR.ToString ();
					}
					break;
				}
			}
		}
	}


	void screenHandle(string type){
		if (!submit&&_lightsOn) {
			switch (type) {
			case "+":
				if (pressure + 1 != 36) {
					Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
					pressure++;
					screenT.text = (string)pressure.ToString ();
				}
				break;
			case "-":
				if (pressure - 1 !=8) {
					Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
					pressure--;
					screenT.text = (string)pressure.ToString ();
				}
				break;
			case "s":
				Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
				if (pressure != gP) {
					this.HandleStrike ();
				} else {
					
					StartCoroutine ("pressureCount");
					submit = true;
					startMano ();
				}
				break;
			}
		}
	}

	void HandleStrike(){
		pressure = 9;
		screenT.text = "--";
		StartCoroutine ("blink");
		submit = false;
		Module.HandleStrike ();
		reachedPressure ();
		T.text = " ";
		BL.text = " ";
		BR.text = " ";

		StopCoroutine ("pressureCount");
		if (timerSound!=null) {
	//		Debug.LogFormat ("CUT SOUND");
			timerSound.StopSound ();
			timerSound = null;
		}
	}

	void HandlePass(){
		StopCoroutine ("pressureCount");
		if (timerSound!=null) {
			//Debug.LogFormat ("CUT SOUND");
			timerSound.StopSound ();
			timerSound = null;
		}	
		Module.HandlePass ();
	}


	public string TwitchHelpMessage = "Submit the target pressure with ''!{0} submit 20''. Set the manometers pressure (TOP = t, BOTTOM LEFT = bl, BOTTOM RIGHT = br) with ''!{0} t 2'' or join them with ''!{0} t 2 bl 3 br 5''. Turn the valve with ''!{0} valve''.";
	KMSelectable[] ProcessTwitchCommand (string command)
	{
		command = command.ToLowerInvariant ().Trim ();

		if (Regex.IsMatch (command, @"^submit \d\d")) {
			pressure = 9;
			List<KMSelectable> plusS = new List<KMSelectable>();

			command = command.Substring (7).Trim ();	
			timer = 60;
			int i = 0;
			if (!(command.Length > 2)) {
				if (int.Parse (command) > 9 && int.Parse (command) < 36) {
					for (i = 0; i < (int.Parse (command) - 9); i++) {
						plusS.Add (sP);
					}
					plusS.Add(screen);
					return plusS.ToArray();
				} else {
					return null;
				}
			}
		} else if(!Regex.IsMatch (command, @"^submit \d\d") && !submit){
				return null;
			}
		
		


		if (Regex.IsMatch (command, @"^valve")) {
			return new[] {abort};
		}

		List<KMSelectable> action = new List<KMSelectable>();
		command += "  ";
		if (submit) {
			if (command.Contains ('t')) {


				string commandT;
				if (command.Length - command.IndexOf ('t') >= 4)
					commandT = command.Substring (command.IndexOf ('t') + 1, 3);
				else
					commandT = command.Substring (command.IndexOf ('t') + 1);

				commandT.Trim ();
				presT = 0;

				//	KMSelectable[] plusT = { sP, sP, sP, sP, sP, sP, sP, sP, sP, sP };
				if(Regex.IsMatch(commandT.Trim(), @"\D?\D")){
					return null;
				}
				for (int i = 0; i < int.Parse (commandT); i++) {
					action.Add (Tp);
				}
			}

			if (command.Contains ("bl")) {

				string commandBL;
				if (command.Length - command.IndexOf ('l') >= 5) {
				//	Debug.LogFormat ("là c'est BL {0}", command);
					commandBL = command.Substring (command.IndexOf ('l') + 1, 3);
			
				//	Debug.LogFormat ("après");
				} else {
				//	Debug.LogFormat ("ge");
					commandBL = command.Substring (command.IndexOf ('l') + 1);
				}
			//	Debug.LogFormat ("{0}", commandBL);
				commandBL.Trim ();
			//	Debug.LogFormat ("{0}", commandBL);
				presBL = 0;

				//KMSelectable[] plusBL = { sP, sP, sP, sP, sP, sP, sP, sP, sP, sP };
				//Debug.LogFormat ("AVANT");
				if(Regex.IsMatch(commandBL.Trim(), @"\D?\D")){
					return null;
				}
				for (int i = 0; i < int.Parse (commandBL); i++) {
					action.Add (BLp);
				}
			//	Debug.LogFormat ("APRES");
			}

			if (command.Contains ("br")) {

				string commandBR;
				//	if (command.Length - command.IndexOf ('r') >= 5) {
			//	Debug.LogFormat ("là c'est le BR  {0}", command);

				commandBR = command.Substring (command.IndexOf ('r') + 1, 3);
				//Debug.LogFormat ("éh oui {0}", commandBR);
				//} else {
				//commandBR = command.Substring (command.IndexOf ('r') + 1);
				//}	
				commandBR.Trim ();
				presBR = 0;

				//KMSelectable[] plusBR = { sP, sP, sP, sP, sP, sP, sP, sP, sP, sP };
				if(Regex.IsMatch(commandBR.Trim(), @"\D?\D")){
					return null;
				}
				for (int i = 0; i < int.Parse (commandBR); i++) {
					action.Add (BRp);
				}
			}

			return action.ToArray ();
		}
		/*if (Regex.IsMatch (command, @"^t +\d\d?")) {
			
			presT = 0;
				command = command.Substring (2).Trim ();

				KMSelectable[] plusT = {sP, sP, sP, sP, sP, sP, sP, sP, sP, sP};
				for (int i = 0; i < int.Parse (command); i++) {
					plusT[i]=Tp;
				}
				return plusT;

		}
*/
		/*
			if (Regex.IsMatch (command, @"^bl +\d\d?")) {
					presBL = 0;
			command = command.Substring (2).Trim ();

			KMSelectable[] plusBL = {sP, sP, sP, sP, sP, sP, sP, sP, sP, sP};
			for (int i = 0; i < int.Parse (command); i++) {
				plusBL[i]=BLp;
			}
			return plusBL;



		}


			if (Regex.IsMatch (command, @"^br +\d\d?")) {
				presBR = 0;
			command = command.Substring (2).Trim ();

			KMSelectable[] plusBR = {sP, sP, sP, sP, sP, sP, sP, sP, sP, sP};
			for (int i = 0; i < int.Parse (command); i++) {
				plusBR[i]=BRp;
			}
			return plusBR;
		}*/
		return null;
	}
	
}