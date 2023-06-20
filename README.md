# Yut.WorldBoss
<div align="center">
   <img width="160" src="/Photos/Yuthung.jpg" alt="logo"></br>   
</div>  

## 简介
这是一个基于Rocket实现的世界BOSS插件   
世界BOSS分四个阶段：      
* 等待刷新阶段
* 报名阶段
* 挑战阶段
* 奖励阶段
## 版权声明
该项目仅供学习使用，未经授权不得使用，修改，分发，二传，销售此插件，如需使用请[联系](#联系方式)作者获得使用权,如果对此插件有什么更好的想法或者其他意见请发邮件[联系](#联系方式)
## 功能
功能|描述
:-:|:-:
定时刷新|世界BOSS会在每天的指定时刻刷新
难度设定|你可以为世界BOSS配置多种难度，并且可以给每天不同的时刻设置不同的难度
门票|世界BOSS开启之后你有一定的时间进行报名，报名需要使用门票
小怪|BOSS不再是单打独斗，有小怪刷新机制
自定义血量|你自定义BOSS以及小怪的血量（上限4,294,967,296）
随机衣服|BOSS和小怪将使用僵尸表中随机的一套衣服
挑战时间|你可以设置挑战的时间，当时间结束还未击杀BOSS则挑战失败
排名|挑战过程实时记录玩家对BOSS的伤害并进行排名
自定义排名奖励|你可以为不同的名次的玩家设置不同的奖励，奖励是随机的，可以设置保底
保底伤害|参与挑战的玩家必须达到指定伤害值才能获得奖励
挑战区域|你可以将BOSS刷新点设置在任何一个可以刷新僵尸的位置
挑战传送|成功报名的玩家挑战过程将复活在挑战区域
BOSS技能|世界BOSS拥有众多技能
自定义技能|你可以自定义技能的名字，半径和伤害，以此实现不同的挑战难度
挑战人数|可以自定义可参与挑战的人数
> 特性：未报名玩家无法对挑战生成的僵尸造成任何伤害，一旦未报名玩家攻击了挑战生成的僵尸将会被传送至随机一个复活点
## 世界BOSS技能
:-:|:-:
burn|当玩家在BOSS附近一定范围内持续受到伤害
thud|将范围内的玩家拉至BOSS周围，BOSS释放地震
shield|开启护盾保护，免疫所有伤害，同时反弹伤害
stomp|随机闪现至范围内一名玩家面前并释放地震
explosion|在范围内所有玩家面前生成火焰爆炸，并造成伤害
explosion2|以BOSS为中心，三个不同半径的圆上生成火焰爆炸，并造成伤害
breath|将范围内所有玩家拉至BOSS面前一条线，BOSS喷火
virus|给范围内所有玩家身上挂上辐射debuff，持续扣辐射值，持续10s
fly|将范围内所有玩家击飞并造成伤害
acid|以BOSS为中心，指定半径的圆上喷吐酸液
acid2|以BOSS为中心，周围6个方向的路径上喷涂酸液
flash|在BOSS头顶和周围三个方向生成闪光眼睛，延迟后造成闪光，可以躲避
heal|回复指定血量
boudler|以BOSS为中心，三个不同半径的圆上生成石头并坠落，砸中后扣血
boulder2|以BOSS为中心，周围6个方向扔出石头
baptism|随机选择范围内一名玩家禁锢在天上，同时在玩家脚底生成闪光眼睛，眼睛爆炸后玩家受到伤害
baptism2|随机选择范围内4名玩家禁锢在天上，同时在玩家脚底生成闪光眼睛，眼睛爆炸后玩家受到伤害
noheadshort|BOSS和小怪的头部伤害和身子伤害一致
## 指令介绍
指令|功能以及用法
:-:|:-:
/wbj|报名阶段使用，用于报名世界BOSS，会检测是否拥有门票
/wbe|挑战阶段使用，用于退出挑战
/wbr|奖励阶段使用，用于领取挑战奖励
/wbti|管理员指令，用于设置世界BOSS刷新时间，用法:/wbti 8 0，表示将刷新时间设置为每天八点整
/wbs|管理员指令，用于在游戏内更改世界BOSS配置
## 僵尸类型
* ACID
* BOSS_ALL
* BOSS_ELECTRIC
* BOSS_ELVER_STOMPER
* BOSS_FIRE
* BOSS_KUWAIT
* BOSS_MAGMA
* BOSS_NUCLEAR
* BOSS_SPIRIT
* BOSS_WIND
* BURNER
* CRAWLER
* DL_BLUE_VOLATILE
* DL_RED_VOLATILE
* FLANKER_FRIENDLY
* FLANKER_STALK
* MEGA
* NORMAL
* SPIRIT
* SPRINTER
## 联系方式
QQ: 1284853081 邮箱: 1284853081@qq.com
> 联系时请备注好说明
