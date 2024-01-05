USE Photo_Gallery

DROP VIEW IF EXISTS `vwphotographydetails`;
CREATE ALGORITHM=UNDEFINED DEFINER = 'root'@'localhost' SQL SECURITY DEFINER VIEW `vwphotographydetails`  AS SELECT `p`.`id` AS `Id`, `p`.`filename` AS `Filename`, `loc`.`_reference` AS `Location`, `p`.`_path` AS `Path`, `p`.`title` AS `Title`, `vw`.`Taglist` AS `Tags` FROM ((`tblphotography` `p` left join `tbllocation` `loc` on((`loc`.`id` = `p`.`location_id`))) left join `vwphotographytags` `vw` on((`vw`.`photography_id` = `p`.`id`)))  ;
