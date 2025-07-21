using UnityEngine;
using UnityEngine.UI;

public class SettingsGame : MonoBehaviour
{
	public enum TypeItemSetting
	{
		Toggle,
		Slider,
		Dropdown
	}

	public Dropdown maxRagdollsDropdown;

	public Toggle shellWeaponToggle;

	public Toggle damageToggle;

	public Toggle effectsHitBulletToggle;

	public Toggle playSound;

	public Toggle renderDetail;

	public Button buttonSave;

	private void Awake()
	{
		if (buttonSave != null)
		{
			maxRagdollsDropdown.value = PlayerPrefsFile.GetInt("maxRagdolls", maxRagdollsDropdown.value);
			damageToggle.isOn = PlayerPrefsFile.GetBool("damage", damageToggle.isOn);
			shellWeaponToggle.isOn = PlayerPrefsFile.GetBool("shells", shellWeaponToggle.isOn);
			effectsHitBulletToggle.isOn = PlayerPrefsFile.GetBool("effectsHitBullet", effectsHitBulletToggle.isOn);
			playSound.isOn = PlayerPrefsFile.GetBool("playSound", playSound.isOn);
			renderDetail.isOn = PlayerPrefsFile.GetBool("renderDetail", renderDetail.isOn);
			buttonSave.onClick.AddListener(Save);
			Save();
		}
	}

	public void Save()
	{
		PlayerPrefsFile.SetInt("maxRagdolls", maxRagdollsDropdown.value);
		ZombieInHome.maxRagdolls = int.Parse(maxRagdollsDropdown.options[maxRagdollsDropdown.value].text);
		TestDummy.maxRagdolls = ZombieInHome.maxRagdolls;
		PlayerPrefsFile.SetBool("damage", damageToggle.isOn);
		DamageOfSprite.enabledComponents = damageToggle.isOn;
		DamageTextureMaterial.enabledComponents = damageToggle.isOn;
		PlayerPrefsFile.SetBool("effectsHitBullet", effectsHitBulletToggle.isOn);
		Blood.max = (effectsHitBulletToggle.isOn ? (ZombieInHome.maxRagdolls * 4) : 0);
		Meat.max = (effectsHitBulletToggle.isOn ? (ZombieInHome.maxRagdolls * 3) : 0);
		WeaponCartridge.effectsHitIsEnabled = effectsHitBulletToggle.isOn;
		PlayerPrefsFile.SetBool("playSound", playSound.isOn);
		AudioListener.pause = !playSound.isOn;
		PlayerPrefsFile.SetBool("renderDetail", renderDetail.isOn);
		RenderDetail.EnableAll();
		if (!renderDetail.isOn)
		{
			RenderDetail.DisableAll();
		}
		PlayerPrefsFile.SetBool("shells", shellWeaponToggle.isOn);
		WeaponHands.shells = shellWeaponToggle.isOn;
		PlayerPrefsFile.Save();
	}
}
