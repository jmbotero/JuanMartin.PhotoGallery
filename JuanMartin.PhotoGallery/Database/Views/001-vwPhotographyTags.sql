USE Photo_Gallery

DROP VIEW IF EXISTS `vwphotographytags`;
CREATE ALGORITHM=UNDEFINED DEFINER = 'root'@'localhost' SQL SECURITY DEFINER VIEW `vwphotographytags`  AS SELECT `p`.`id` AS `photography_id`, group_concat(distinct `t`.`word` separator ',') AS `Taglist` FROM ((`tblphotography` `p` join `tblphotographytags` `pt` on((`pt`.`photography_id` = `p`.`id`))) join `tbltag` `t` on((`t`.`id` = `pt`.`tag_id`))) GROUP BY `p`.`id`;
