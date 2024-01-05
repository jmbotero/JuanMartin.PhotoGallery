USE Photo_Gallery

DROP VIEW IF EXISTS `vwphotographywithranking`;
CREATE ALGORITHM=UNDEFINED DEFINER = 'root'@'localhost' SQL SECURITY DEFINER VIEW `vwphotographywithranking`  AS SELECT `p`.`Id` AS `Id`, `p`.`Filename` AS `Filename`, ifnull(`p`.`Location`,'') AS `Location`, `p`.`Path` AS `Path`, (case when (locate('slide',`p`.`Path`) > 0) then 1 else 0 end) AS `Source`, `p`.`Title` AS `Title`, ifnull(`p`.`Tags`,'') AS `Tags`, ifnull(`r`.`_rank`,0) AS `Rank`, ifnull(`udfGetAverageRank`(`p`.`Id`),0) AS `AverageRank` FROM (`vwphotographydetails` `p` left join `tblranking` `r` on(((`r`.`user_id` = `p1`()) and (`r`.`photography_id` = `p`.`Id`))))  ;
