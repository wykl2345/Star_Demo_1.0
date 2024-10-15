VAR player_name = "" 
VAR charTex_path = "" 
VAR backTex_path = "" 
-> part0
= part0
-> part1
= part1
->part1_0
= part1_0
~ player_name = "puppet"
~ charTex_path = ""
~ backTex_path = ""
来找本法师干什么啊？
-> part2
= part2
~ player_name = "player"
~ charTex_path = ""
~ backTex_path = ""
+ [请告诉我如何耕地]
-> part3
+ [你这里有什么]
-> part5
= part3
->part3_0
= part3_0
~ player_name = "puppet"
~ charTex_path = ""
~ backTex_path = ""
拿起“锄头”对着空地右键就能将其转变为耕地
+[继续] ->part3_1
->part3_1
= part3_1
~ player_name = "puppet"
~ charTex_path = ""
~ backTex_path = ""
拿着种子对着空闲的耕地就可以右键种下
+[继续] ->part3_2
->part3_2
= part3_2
~ player_name = "puppet"
~ charTex_path = ""
~ backTex_path = ""
记得拿水壶保持它的水分充足
-> END
= part4
->part4_0
= part4_0
~ player_name = "puppet"
~ charTex_path = ""
~ backTex_path = ""
来看看吧
+[继续]-> part5
= part5
->part5_0
= part5_0
~ player_name = "puppet"
~ charTex_path = ""
~ backTex_path = ""
Event.Open_Shop
-> END
