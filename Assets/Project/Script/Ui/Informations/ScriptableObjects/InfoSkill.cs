using UnityEngine;

[CreateAssetMenu(fileName = "InfoSkill", menuName = "Skills/TestSkill")]
public class InfoSkill : InfoObject
{
	//효과를 담을 리스트를 만들기로 해놓기
	//스킬 습득 조건을 리스트로 만들기로 해놓기
	public int requireLevel;
	public int maxStack = 1;
	public float cooldown;
}
