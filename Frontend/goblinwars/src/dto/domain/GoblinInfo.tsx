import { StatusEnum } from "../enum/StatusEnum";
import { GoblinEquipmentInfo } from "./GoblinEquipmentInfo";
import { GoblinMining } from "./GoblinMining";
import { GoblinSkillInfo } from "./GoblinSkillInfo";
import { SkillDetailInfo } from "./SkillDetailInfo";

export interface GoblinInfo {
  id:number;
  idToken: number;
  //tokenId: number;
  iduser: number;
  nameUser: string;
  userAddress: string;
  idTokenFather?: number;
  idTokenMother?: number;
  idTokenSpouse?: number;
  birthday: string;
  cooldownDate: string;
  inCooldown: boolean;
  lastuserchange: string;
  xp: number;
  name: string;
  headImageURL: string;
  imageURL: string;
  genre: string;
  race: number;
  raceName: string;
  hair: number;
  hairName: string;
  ear: number;
  earName: string;
  eye: number;
  eyeName: string;
  mount: number;
  mountName: string;
  skin: number;
  skinName: string;
  haircolor: string;
  skincolor: string;
  eyecolor: string;
  strength: number;
  agility: number;
  vigor: number;
  intelligence: number;
  charism: number;
  perception: number;
  busy?: string;
  isbusy: boolean;
  status: StatusEnum;
  rarity: number;
  rarityenum: number;
  sonscount: number;
  isavaliable: boolean;
  minted: boolean;
  features?: any[];
  goblinMining?: GoblinMining;
  sprite: string;
  spritetired: string;
  goblinEquipment: GoblinEquipmentInfo;
  goblinSkillList: GoblinSkillInfo;
  questaffinity: number;
} 