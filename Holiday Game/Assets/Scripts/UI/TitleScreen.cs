using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public Button play, options, quit;
    public TMP_Text title, characterChoiceText, nameObject, descObject;
    public Image characterSlot1, characterSlot2, characterSlot3, characterSlot4;
    public Image characterPortrait;
    private bool hitPlay = false;
    private float animationSpeedP = -500f;
    private float animationSpeedO = -500f;
    private float animationSpeedQ = -500f;
    private float animationSpeedT = -750f;
    private float animationSpeedCT = 2000f;
    private float animationSpeedCS1 = 2000f;
    private float animationSpeedCS2 = 2000f;
    private float animationSpeedCS3 = 2000f;
    private float animationSpeedCS4 = 2000f;
    private float animationAccel = 5000f;

    private void Start()
    {
        characterPortrait.gameObject.SetActive(false);
        nameObject.gameObject.SetActive(false);
        descObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        // Doing play animation
        if (hitPlay)
        {
            bool done1 = true;
            if (animationSpeedP < 5000f)
            {
                animationSpeedP += animationAccel * delta;
            }

            RectTransform rectP = play.gameObject.GetComponent<RectTransform>();
            RectTransform rectO = options.gameObject.GetComponent<RectTransform>();
            RectTransform rectQ = quit.gameObject.GetComponent<RectTransform>();
            RectTransform rectT = title.gameObject.GetComponent<RectTransform>();
            RectTransform rectCT = characterChoiceText.gameObject.GetComponent<RectTransform>();
            RectTransform rectSlot1 = characterSlot1.gameObject.GetComponent<RectTransform>();
            RectTransform rectSlot2 = characterSlot2.gameObject.GetComponent<RectTransform>();
            RectTransform rectSlot3 = characterSlot3.gameObject.GetComponent<RectTransform>();
            RectTransform rectSlot4 = characterSlot4.gameObject.GetComponent<RectTransform>();

            if (rectP.localPosition.x > -500)
            {
                rectP.SetPositionAndRotation(new Vector3(rectP.position.x - delta * animationSpeedP, rectP.position.y, 0), Quaternion.identity);
                done1 = false;
            }

            if (rectP.position.x < 100)
            {
                if (animationSpeedO < 5000f)
                {
                    animationSpeedO += animationAccel * delta;
                }
                if (rectO.localPosition.x > -500)
                {
                    rectO.SetPositionAndRotation(new Vector3(rectO.position.x - delta * animationSpeedO, rectO.position.y, 0), Quaternion.identity);
                    done1 = false;
                }
            }

            if (rectO.position.x < 100)
            {
                if (animationSpeedQ < 5000f)
                {
                    animationSpeedQ += animationAccel * delta;
                }
                if (rectQ.localPosition.x > -500)
                {
                    rectQ.SetPositionAndRotation(new Vector3(rectQ.position.x - delta * animationSpeedQ, rectQ.position.y, 0), Quaternion.identity);
                    done1 = false;
                }
            }

            if (rectT.position.y < 1000 * GetComponent<RectTransform>().lossyScale.y)
            {
                animationSpeedT += animationAccel * delta;
                rectT.SetPositionAndRotation(new Vector3(rectT.position.x, rectT.position.y + delta * animationSpeedT, 0), Quaternion.identity);
                done1 = false;
            }

            if (done1)
            {
                float minSpeed = 750;
                float maxSpeed = 3000;

                if (rectCT.localPosition.y > 175f)
                {
                    animationSpeedCT = 2000 * ((rectCT.localPosition.y - 175f) / 175f);
                    if (animationSpeedCT < minSpeed)
                    {
                        animationSpeedCT = minSpeed;
                    }
                    rectCT.SetPositionAndRotation(new Vector3(rectCT.position.x, rectCT.position.y - animationSpeedCT * delta, 0), Quaternion.identity);
                }
                else
                {
                    rectCT.localPosition.Set(rectCT.position.x, 175f, 0);
                }

                if (rectSlot1.localPosition.y < 75f)
                {
                    animationSpeedCS1 = maxSpeed * (-(rectSlot1.localPosition.y - 75) / 375f);
                    if (animationSpeedCS1 < minSpeed)
                    {
                        animationSpeedCS1 = minSpeed;
                    }
                    rectSlot1.SetPositionAndRotation(new Vector3(rectSlot1.position.x, rectSlot1.position.y + animationSpeedCS1 * delta, 0), Quaternion.identity);
                }
                else
                {
                    rectSlot1.localPosition.Set(rectSlot1.position.x, 75f, 0);
                }

                if (rectSlot2.localPosition.y < 0f && rectSlot1.localPosition.y > -250)
                {
                    animationSpeedCS2 = maxSpeed * (-(rectSlot2.localPosition.y) / 375f);
                    if (animationSpeedCS2 < minSpeed)
                    {
                        animationSpeedCS2 = minSpeed;
                    }
                    rectSlot2.SetPositionAndRotation(new Vector3(rectSlot2.position.x, rectSlot2.position.y + animationSpeedCS2 * delta, 0), Quaternion.identity);
                }
                else
                {
                    rectSlot2.localPosition.Set(rectSlot2.position.x, 0f, 0f);
                }

                if (rectSlot3.localPosition.y < -75f && rectSlot2.localPosition.y > -325)
                {
                    animationSpeedCS3 = maxSpeed * (-(rectSlot3.localPosition.y + 75f) / 375f);
                    if (animationSpeedCS3 < minSpeed)
                    {
                        animationSpeedCS3 = minSpeed;
                    }
                    rectSlot3.SetPositionAndRotation(new Vector3(rectSlot3.position.x, rectSlot3.position.y + animationSpeedCS3 * delta, 0), Quaternion.identity);
                }
                else
                {
                    rectSlot3.localPosition.Set(rectSlot3.position.x, -75f, 0f);
                }

                if (rectSlot4.localPosition.y < -150f && rectSlot3.localPosition.y > -400)
                {
                    animationSpeedCS4 = maxSpeed * (-(rectSlot4.localPosition.y + 150f) / 375f);
                    if (animationSpeedCS4 < minSpeed)
                    {
                        animationSpeedCS4 = minSpeed;
                    }
                    rectSlot4.SetPositionAndRotation(new Vector3(rectSlot4.position.x, rectSlot4.position.y + animationSpeedCS4 * delta, 0), Quaternion.identity);
                }
                else
                {
                    rectSlot4.localPosition.Set(rectSlot4.position.x, -150f, 0f);
                }
            }
        }
    }

    public void PressPlay()
    {
        hitPlay = true;
    }

    public void SelectCharacter(int slot)
    {
        if (slot == 1)
        {
            GameManager.instance.StartGame(ResourceManager.UpgradeIndex.CandyCornWeaponUpgrade);
        }
    }
}
