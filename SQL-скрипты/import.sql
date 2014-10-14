TRUNCATE TABLE _not_privatiz_premises1;

INSERT INTO _not_privatiz_premises1
  (street_prefix,street,house,house_premises_count,crash_bit,flat_num_checked_in_mp,
  total_area,total_flat_count,description,commercial_fund,special_fund, id_region)
SELECT *, 1
FROM _not_privatiz_premises_bikey nppb
  UNION ALL
SELECT *, 2
FROM _not_privatiz_premises_central
  UNION ALL
SELECT *, 3
FROM _not_privatiz_premises_energetic nppe
  UNION ALL
SELECT *, 4
FROM _not_privatiz_premises_gidrostroy nppg
  UNION ALL
SELECT *, 5
FROM _not_privatiz_premises_osinovka nppo
  UNION ALL
SELECT *, 6
FROM _not_privatiz_premises_padun nppp
  UNION ALL
SELECT *, 7
FROM _not_privatiz_premises_porozhki nppp
  UNION ALL
SELECT *, 8
FROM _not_privatiz_premises_suhoy npps;


-- Тут необходимо вручную поправить все пометки о старых названиях улиц, их мало. Ставлю в поле description с маркером "||" 
-- и указанием id строки, из которой было вручную перенесено. 
-- Ищутся по запросам:
SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%ранее%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%новостройка%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%снесен%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%сгорел%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%п. Заярский%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%Осиновка%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%техпаспорт%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%совет №%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%дом%');

SELECT *
FROM _not_privatiz_premises1 npp
WHERE (npp.street LIKE '%т/п%');

SELECT *
FROM _not_privatiz_premises1 npp1
WHERE npp1.street = 'Тайшетская (Заярский)';

-- ул. Мало-Амурская 60 - дом сгорел, пометки необходимо переносить вручную
-- ул. Южная 7б имеет уникальную пометку "не разобран, проживают (на 17.04.14)", перенести в description

-- Проверка названий улиц

SELECT DISTINCT street
FROM _not_privatiz_premises1 npp
WHERE street IS NOT NULL AND street <> 'ИТОГО' AND street <> '';

-- Корректировка некоторых названий
UPDATE _not_privatiz_premises1
set street = '50 лет Октября'
  WHERE street = ' 50 лет Октября';
  
UPDATE _not_privatiz_premises1
set street = 'XX Партсъезда'
  WHERE street = 'ХХ Партсъезда';
  
-- Добавление пропущенных префиксов ул. (предполагается, что все префиксы - улицы, т.е. надо проверять по базе вручную так ли это)

UPDATE _not_privatiz_premises1
set street_prefix = 'ул.'
WHERE street IS NOT NULL AND street <> 'ИТОГО' AND street <> '' AND street_prefix IS NULL;

SELECT id_record, street_prefix, street, id_region
FROM _not_privatiz_premises1 npp
WHERE street IS NOT NULL AND street <> 'ИТОГО' AND street <> '' AND street_prefix IS NULL;

-- Список адресов, которые не удалось автоматом подвязать к klader'у
-- 2300 домов
-- 183 не подвязались автоматом
SELECT id_record, CONCAT(region,', ',CONCAT(a.street_prefix,' ', a.street)) AS street
FROM (
SELECT id_record, street_prefix, street, 
  CASE id_region 
    WHEN 1 THEN 'жилрайон. Бикей' 
    WHEN 2 THEN 'жилрайон. Центральный' 
    WHEN 3 THEN 'жилрайон. Энергетик' 
    WHEN 4 THEN 'жилрайон. Гидростроитель' 
    WHEN 5 THEN 'жилрайон. Осиновка' 
    WHEN 6 THEN 'жилрайон. Падун' 
    WHEN 7 THEN 'жилрайон. Порожский' 
    WHEN 8 THEN 'жилрайон. Сухой' 
    ELSE id_region END AS region
FROM _not_privatiz_premises1 npp
WHERE street IS NOT NULL AND street <> 'ИТОГО' AND street <> '') a
WHERE CONCAT(region,', ',CONCAT(a.street_prefix,' ', a.street)) NOT IN (
  SELECT street_name FROM v_kladr_streets vks);
  
-- Правка названий улиц и префиксов
UPDATE _not_privatiz_premises1
set street = 'К.Маркса'
WHERE street = 'К. Маркса';

UPDATE _not_privatiz_premises1
set street_prefix = 'б-р.'
WHERE street = 'Космонавтов' AND id_region = 2;

UPDATE _not_privatiz_premises1
set street_prefix = 'пр-кт.'
WHERE street = 'Ленина' AND id_region = 2;

UPDATE _not_privatiz_premises1
set street_prefix = 'б-р.'
WHERE street = 'Победы' AND id_region = 2;

UPDATE _not_privatiz_premises1
set street = 'Воинов-интернационалистов'
WHERE street = 'Воинов-интернацион.' AND id_region = 3;

UPDATE _not_privatiz_premises1
set street = 'Иванова'
WHERE street = 'Иванова, Иванова В.В.' AND id_region = 3;

UPDATE _not_privatiz_premises1
set street = 'Звездный 2-й'
WHERE street = '2-й Звездный' AND id_region = 4;

UPDATE _not_privatiz_premises1
set street = '20 Партсъезда'
WHERE street = 'XX Партсъезда' AND id_region = 7;

UPDATE _not_privatiz_premises1
set street = 'В.Синицы'
WHERE street = 'В. Синицы' AND id_region = 6;

UPDATE _not_privatiz_premises1
set street = 'Парковая 2-я'
WHERE street = '2-я Парковая' AND id_region = 6;

UPDATE _not_privatiz_premises1
set street = 'Рабочий 3-й'
WHERE street = 'З-й Рабочий' AND id_region = 6;

UPDATE _not_privatiz_premises1
set street_prefix = 'туп.'
WHERE street_prefix = 'тупик/ ул.' AND street = 'Крылатый' AND id_region = 2;

-- Список домов без номера дома, этим домам заданы невалидные идентификаторы undX, где X - порядковый номер, все найдено 3 таких дома
SELECT *
FROM _not_privatiz_premises1 npp
WHERE (house IS NULL OR house = '') AND (flat_num_checked_in_mp IS NULL OR flat_num_checked_in_mp = '')
  AND UPPER(street) != 'ИТОГО';
  
--Экспортим квартиры (только квартиры с указанием адреса) в таблицу _not_privatiz_premises2
-- 9850 квартир
 CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.reorganize_premises_proc()
BEGIN 
  -- Объявляем колонки таблицы
Declare id_record INT;
Declare street_prefix VARCHAR(255);
Declare street VARCHAR(255);
Declare house VARCHAR(255);
Declare house_premises_count VARCHAR(255);
Declare crash_bit VARCHAR(255);
Declare flat_num_checked_in_mp VARCHAR(255);
Declare total_area VARCHAR(255);
Declare total_flat_count VARCHAR(255);
Declare description VARCHAR(255);
Declare commercial_fund VARCHAR(255);
Declare special_fund VARCHAR(255);
Declare id_region INT;
DECLARE evaluated bool DEFAULT FALSE;
  -- Объявляем вспомогательные буферы
Declare street_prefix_buff VARCHAR(255) DEFAULT '';
Declare street_buff VARCHAR(255) DEFAULT '';
Declare house_buff VARCHAR(255) DEFAULT '';
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT *
FROM _not_privatiz_premises1 npp;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_record,
    street_prefix,
    street,
    house,
    house_premises_count,
    crash_bit,
    flat_num_checked_in_mp,
    total_area,
    total_flat_count,
    description,
    commercial_fund,
    special_fund,
    id_region;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  -- TODO
  SET evaluated = FALSE;
  -- Если это строка здания
  IF ((street IS NOT NULL AND TRIM(street) <> '' AND
      house IS NOT NULL AND TRIM(house) <> '' AND 
      street_prefix IS NOT NULL AND TRIM(street_prefix) <> '') 
    AND (
    (TRIM(street) <> street_buff) || 
    (TRIM(house) <> house_buff) || 
    (TRIM(street_prefix) <> street_prefix_buff)) AND 
    (UPPER(TRIM(street)) <> 'ИТОГО')) THEN
      SET street_buff = TRIM(street);
      SET house_buff = TRIM(house);
      SET street_prefix_buff = TRIM(street_prefix);
      -- Если в данной же строке нет информации об единственной квартире дома, то закончить расчеты
      IF ((flat_num_checked_in_mp IS NULL) OR (TRIM(flat_num_checked_in_mp) = '')) THEN
        set evaluated = TRUE;
      END IF;
  ELSEIF (UPPER(TRIM(street)) = 'ИТОГО') THEN
    set evaluated = TRUE;
  END IF;
  
  IF ((street_buff IS NULL) OR (TRIM(street_buff) = '') OR
      (street_prefix_buff IS NULL) OR (TRIM(street_prefix_buff) = '') OR
      (house_buff IS NULL) OR (TRIM(house_buff) = '')) THEN
    SIGNAL SQLSTATE 'ERR0R' SET MESSAGE_TEXT = street_buff;
  END IF;

  IF ((flat_num_checked_in_mp IS NOT NULL) AND (TRIM(flat_num_checked_in_mp) <> '')) THEN
    INSERT INTO _not_privatiz_premises2
    VALUES(id_record,
      TRIM(street_prefix_buff),
      TRIM(street_buff),
      TRIM(house_buff),
      TRIM(house_premises_count),
      TRIM(crash_bit),
      TRIM(flat_num_checked_in_mp),
      TRIM(total_area),
      TRIM(total_flat_count),
      TRIM(description),
      TRIM(commercial_fund),
      TRIM(special_fund),
      TRIM(id_region));
    SET evaluated = TRUE;
  END IF;
  IF (evaluated = false) THEN
    SIGNAL SQLSTATE 'ERR0R' SET MESSAGE_TEXT = street_buff;
  END IF;
END WHILE;
CLOSE cursor1;
-- Проверяем результат и откатываемся
SELECT *
FROM _not_privatiz_premises2 npp;
COMMIT;
END

-- Правим колонку площадей total_area
UPDATE _not_privatiz_premises2
set total_area = SUBSTR(total_area, 1, CHARACTER_LENGTH(total_area) - 1)
WHERE total_area LIKE '%.';

UPDATE _not_privatiz_premises2
set total_area = REPLACE(total_area, '.', ',');

-- В поле total_flat_count id_record = 1475 занесено значение 0,5 (пол помещения является муниципальным)

-- Корректируем номер помещения
-- Список невалидных номеров помещений:
select id_record, flat_num_checked_in_mp 
from _not_privatiz_premises2 
where flat_num_checked_in_mp NOT REGEXP '^[0-9]+[а]*$';

-- Вручную исправил несколько номеров, импортировавшихся как дата
-- id_record = 1163, 55 -64 - таких квартир не существует, запись удалена

-- Выборка проблемных квартир
SELECT *
FROM (
select id_record,street_prefix,street, house, CASE id_region 
    WHEN 1 THEN 'жилрайон. Бикей' 
    WHEN 2 THEN 'жилрайон. Центральный' 
    WHEN 3 THEN 'жилрайон. Энергетик' 
    WHEN 4 THEN 'жилрайон. Гидростроитель' 
    WHEN 5 THEN 'жилрайон. Осиновка' 
    WHEN 6 THEN 'жилрайон. Падун' 
    WHEN 7 THEN 'жилрайон. Порожский' 
    WHEN 8 THEN 'жилрайон. Сухой' 
    ELSE id_region END AS region, flat_num_checked_in_mp 
from _not_privatiz_premises2 
where flat_num_checked_in_mp NOT REGEXP '^[0-9]+[аб]*$') a

-- id661, комнаты 2,3,4 - могут сдавать раздельно

UPDATE _not_privatiz_premises2
set flats_prepair = SUBSTRING(flat_num_checked_in_mp, 1, LOCATE('(',flat_num_checked_in_mp) - 1),
  rooms = SUBSTRING(flat_num_checked_in_mp, LOCATE('(',flat_num_checked_in_mp) + 1, 
  LOCATE(')',flat_num_checked_in_mp) - LOCATE('(',flat_num_checked_in_mp) - 1)
WHERE flat_num_checked_in_mp NOT REGEXP '^[0-9]+[аб]*$' AND flat_num_checked_in_mp NOT LIKE '%|%'
  AND flat_num_checked_in_mp LIKE '%(%';
  
-- Список квартир, расположенных в нескольких строках таблицы (по-комнатно)
SELECT npp.id_record, npp.street_prefix, npp.street, npp.house, npp.id_region, flats_prepair, rooms, npp.total_area, npp.description
, npp.flat_num_checked_in_mp, npp.commercial_fund, npp.special_fund, npp.crash_bit
FROM _not_privatiz_premises2 npp
WHERE flats_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _not_privatiz_premises2 npp2
   WHERE npp.street_prefix = npp2.street_prefix AND
    npp.street = npp2.street AND npp.house = npp2.house 
    AND npp.id_region = npp2.id_region AND
    npp.flats_prepair = npp2.flats_prepair) > 1
	
-- Вставляем в таблицу комнат непроблемные комнаты
INSERT INTO _not_privatiz_rooms1 (id_premises, sub_premises_number, total_area, description, id_fund_type)
SELECT npp.id_record, rooms, npp.total_area, npp.description
, IF (npp.commercial_fund = 1, 2, IF (npp.special_fund = 1, 3, NULL))
FROM _not_privatiz_premises2 npp
WHERE flats_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _not_privatiz_premises2 npp2
   WHERE npp.street_prefix = npp2.street_prefix AND
    npp.street = npp2.street AND npp.house = npp2.house 
    AND npp.id_region = npp2.id_region AND
    npp.flats_prepair = npp2.flats_prepair) = 1
	
-- Вставляем в таблицу комнат проблемные комнаты
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.reorganize_problem_rooms_proc()
BEGIN 
  -- Объявляем колонки таблицы
Declare id_record INT;
Declare street_prefix VARCHAR(255);
Declare street VARCHAR(255);
Declare house VARCHAR(255);
Declare total_area VARCHAR(255);
Declare description VARCHAR(255);
Declare commercial_fund VARCHAR(255);
Declare special_fund VARCHAR(255);
Declare flat VARCHAR(255);
Declare room_number VARCHAR(255);
Declare id_region INT;
  -- Объявляем вспомогательные буферы
Declare id_record_buff INT DEFAULT 0;
Declare street_prefix_buff VARCHAR(255) DEFAULT '';
Declare street_buff VARCHAR(255) DEFAULT '';
Declare house_buff VARCHAR(255) DEFAULT '';
Declare flat_buff VARCHAR(255) DEFAULT '';
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT npp.id_record, npp.street_prefix, npp.street, npp.house, npp.id_region, flats_prepair, rooms, npp.total_area, npp.description
, npp.commercial_fund, npp.special_fund
FROM _not_privatiz_premises2 npp
WHERE flats_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _not_privatiz_premises2 npp2
   WHERE npp.street_prefix = npp2.street_prefix AND
    npp.street = npp2.street AND npp.house = npp2.house 
    AND npp.id_region = npp2.id_region AND
    npp.flats_prepair = npp2.flats_prepair) > 1;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_record,
    street_prefix,
    street,
    house,
    id_region,
    flat,
    room_number,
    total_area,
    description,
    commercial_fund,
    special_fund;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  IF ((TRIM(street) <> street_buff) || 
    (TRIM(house) <> house_buff) || 
    (TRIM(street_prefix) <> street_prefix_buff) || 
    (TRIM(flat) <> flat_buff)) THEN
      SET street_buff = TRIM(street);
      SET house_buff = TRIM(house);
      SET street_prefix_buff = TRIM(street_prefix);
      SET id_record_buff = TRIM(id_record);
      SET flat_buff = TRIM(flat);
  END IF;
  
  IF ((street_buff IS NULL) OR (TRIM(street_buff) = '') OR
      (street_prefix_buff IS NULL) OR (TRIM(street_prefix_buff) = '') OR
      (house_buff IS NULL) OR (TRIM(house_buff) = '') OR
      (flat_buff IS NULL) OR (TRIM(flat_buff) = '') OR
      (id_record_buff IS NULL) OR (TRIM(id_record_buff) = '')) THEN
    SIGNAL SQLSTATE 'ERR0R' SET MESSAGE_TEXT = street_buff;
  ELSE
    INSERT INTO _not_privatiz_rooms1 (id_premises, sub_premises_number, total_area, 
      description, id_fund_type)
    VALUES(id_record_buff,
      TRIM(room_number),
      TRIM(total_area),
      TRIM(description),
      TRIM(IF (commercial_fund = 1, 2, IF (special_fund = 1, 3, NULL))));
  END IF;
END WHILE;
CLOSE cursor1;
COMMIT;
END

--Переносим квартиры на следующий шаг
INSERT INTO _not_privatiz_premises3
SELECT id_record, street_prefix, street, house, house_premises_count,
  crash_bit, REPLACE(flat_num_checked_in_mp,'|',','), IFNULL(total_area, 0), IFNULL(total_flat_count, 0),
  description, IF (commercial_fund = 1, 2, IF (special_fund = 1, 3, NULL)), id_region
FROM _not_privatiz_premises2
WHERE flats_prepair IS NULL;

INSERT INTO _not_privatiz_premises3
SELECT id_record, street_prefix, street, house, house_premises_count,
  crash_bit, flats_prepair, IFNULL(total_area, 0), IFNULL(total_flat_count, 0),
  description, IF (commercial_fund = 1, 2, IF (special_fund = 1, 3, NULL)), id_region
FROM _not_privatiz_premises2 npp
WHERE flats_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _not_privatiz_premises2 npp2
   WHERE npp.street_prefix = npp2.street_prefix AND
    npp.street = npp2.street AND npp.house = npp2.house 
    AND npp.id_region = npp2.id_region AND
    npp.flats_prepair = npp2.flats_prepair) = 1

-- 82 комнаты, 31 квартира,
INSERT INTO _not_privatiz_premises3
SELECT id_record, street_prefix, street, house, house_premises_count,
  crash_bit, flats_prepair, IFNULL(total_area, 0), IFNULL(total_flat_count, 0),
  description, IF (commercial_fund = 1, 2, IF (special_fund = 1, 3, NULL)), id_region
FROM (
SELECT *
FROM _not_privatiz_premises2 npp
WHERE flats_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _not_privatiz_premises2 npp2
   WHERE npp.street_prefix = npp2.street_prefix AND
    npp.street = npp2.street AND npp.house = npp2.house 
    AND npp.id_region = npp2.id_region AND
    npp.flats_prepair = npp2.flats_prepair) > 1
GROUP BY npp.street_prefix, npp.street, npp.house, flats_prepair, npp.id_region) a

-- Тримим комнаты, дублируем все строки
-- Всего стало 168 комнат, на 30 больше, чем было до нормализации. Площади оставлены только у первых комнат, добавлен комментарий
-- Бывшие групповые комнаты помечены в описании меткой ||gid X, где X - номер группы, к которой раньше относилась комната
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.normalize_rooms_numbers_proc()
BEGIN 
  -- Объявляем колонки таблицы
Declare id_record INT;
Declare id_premises INT;
Declare sub_premises_number_list VARCHAR(255);
Declare sub_premises_number_current VARCHAR(255);
Declare total_area VARCHAR(255);
Declare description VARCHAR(255);
Declare id_fund_type INT;
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT *
      FROM _not_privatiz_rooms1
      WHERE sub_premises_number REGEXP ',+';
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_record,
    id_premises,
    sub_premises_number_list,
    total_area,
    description,
    id_fund_type;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE sub_premises_number_list <> '' DO
    set sub_premises_number_current = 
      SUBSTRING(sub_premises_number_list, 1, LOCATE(',',sub_premises_number_list) - 1);
    IF (sub_premises_number_current = '') THEN
      set sub_premises_number_current = sub_premises_number_list;
      set sub_premises_number_list = '';
    ELSE 
      set sub_premises_number_list = 
        SUBSTRING(sub_premises_number_list, LOCATE(',',sub_premises_number_list) + 1);
    END IF;
    INSERT INTO _not_privatiz_rooms2(id_premises, sub_premises_number, total_area, description,
      id_fund_type)
    VALUES (id_premises, sub_premises_number_current, total_area, description, id_fund_type);
  END WHILE;
END WHILE;
CLOSE cursor1;
COMMIT;
END

INSERT INTO _not_privatiz_rooms2(id_premises, sub_premises_number, total_area,
  description,id_fund_type)
SELECT id_premises, 
  sub_premises_number,
  total_area, description, id_fund_type
FROM _not_privatiz_rooms1
WHERE sub_premises_number NOT LIKE '%,%'

-- делаем правки номеров домов
UPDATE _not_privatiz_premises3
set house = TRIM(house);

UPDATE _not_privatiz_premises3
set house = LOWER(house);

UPDATE _not_privatiz_premises3
set house = '103'
WHERE house = 'und3';

UPDATE _not_privatiz_premises3
set house = '23'
WHERE house = 'und1';

UPDATE _not_privatiz_premises3
set house = '25'
WHERE house = 'und2';
----------------------------------
-- Нормализуем данные о зданиях --
----------------------------------
-- Избавляемся от нескольких полей примечаний
UPDATE _not_privatiz_houses_central
set Примечание =
 CONCAT(Примечание,
  IF(Примечание2 IS NOT NULL AND TRIM(Примечание2) <> '', CONCAT(' || ',Примечание2), ''),
  IF(Примечание3 IS NOT NULL AND TRIM(Примечание3) <> '', CONCAT(' || ',Примечание3), ''));
  
UPDATE _not_privatiz_houses_energetic
set Примечание =
 CONCAT(Примечание,
  IF(Column1 IS NOT NULL AND TRIM(Column1) <> '', CONCAT(' || ',Column1), ''),
  IF(Column2 IS NOT NULL AND TRIM(Column2) <> '', CONCAT(' || ',Column2), ''));
  
UPDATE _not_privatiz_houses_gidrostroy
set Примечание =
 CONCAT(Примечание,
  IF(Column1 IS NOT NULL AND TRIM(Column1) <> '', CONCAT(' || ',Column1), ''));
  
UPDATE _not_privatiz_houses_osinovka
set Примечание =
 CONCAT(Примечание,
  IF(Column1 IS NOT NULL AND TRIM(Column1) <> '', CONCAT(' || ',Column1), ''));

UPDATE _not_privatiz_houses_porozhki
set Примечание =
 CONCAT(Примечание,
  IF(Column1 IS NOT NULL AND TRIM(Column1) <> '', CONCAT(' || ',Column1), ''));
  
UPDATE _not_privatiz_houses_suhoy
set Примечание =
 CONCAT(Примечание,
  IF(Column1 IS NOT NULL AND TRIM(Column1) <> '', CONCAT(' || ',Column1), ''));
  
-- Объединеям таблицы
INSERT INTO _not_privatiz_houses1
SELECT *, 1
FROM _not_privatiz_houses_bikey nppb
  UNION ALL
SELECT *, 2
FROM _not_privatiz_houses_central
  UNION ALL
SELECT *, 3
FROM _not_privatiz_houses_energetic nppe
  UNION ALL
SELECT *, 4
FROM _not_privatiz_houses_gidrostroy nppg
  UNION ALL
SELECT *, 5
FROM _not_privatiz_houses_osinovka nppo
  UNION ALL
SELECT *, 6
FROM _not_privatiz_houses_padun nppp
  UNION ALL
SELECT *, 7
FROM _not_privatiz_houses_porozhki nppp
  UNION ALL
SELECT *, 8
FROM _not_privatiz_houses_suhoy npps;

-- Корректируем названия улиц
SELECT *
FROM _not_privatiz_houses1
WHERE street LIKE '%заярск%';

SELECT *
FROM _not_privatiz_houses1
WHERE street LIKE '%осиновк%';

UPDATE _not_privatiz_houses1
set street = 'XX Партсъезда'
  WHERE street = 'ХХ Партсъезда';

UPDATE _not_privatiz_houses1
set street = 'В.Синицы'
WHERE street = 'В. Синицы' AND id_region = 6;

UPDATE _not_privatiz_houses1
set street = '20 Партсъезда'
WHERE street = 'XX Партсъезда' AND id_region = 7;

UPDATE _not_privatiz_houses1
set street = 'Парковая 2-я'
WHERE street = '2-я Парковая' AND id_region = 6;

UPDATE _not_privatiz_houses1
set street = 'Рабочий 3-й'
WHERE street = 'З-й Рабочий' AND id_region = 6;

UPDATE _not_privatiz_houses1
set street = 'Звездный 2-й'
WHERE street = '2-й Звездный' AND id_region = 4;

UPDATE _not_privatiz_houses1
set street = 'Иванова'
WHERE street = 'Иванова, Иванова В.В.' AND id_region = 3;

UPDATE _not_privatiz_houses1
set street_prefix = 'б-р.'
WHERE street = 'Победы' AND id_region = 2;

UPDATE _not_privatiz_houses1
set street_prefix = 'пр-кт.'
WHERE street = 'Ленина' AND id_region = 2;

UPDATE _not_privatiz_houses1
set street_prefix = 'б-р.'
WHERE street = 'Космонавтов' AND id_region = 2;

UPDATE _not_privatiz_houses1
set street = 'К.Маркса'
WHERE street = 'К. Маркса';

UPDATE _not_privatiz_houses1
set street = 'Воинов-интернационалистов'
WHERE street = 'Воинов-интернацион.' AND id_region = 3;

UPDATE _not_privatiz_houses1
set street = 'Маршала Жукова'
WHERE street = 'Марш. Жукова';

UPDATE _not_privatiz_houses1
set street = 'Новосибирская'
WHERE street = 'Н-Сибирская' AND id_region = 4;

UPDATE _not_privatiz_houses1
set street = 'Энергетическая 1-я'
WHERE street = '1-я Энергетическая' AND id_region = 6;

-- Правим префиксы

UPDATE _not_privatiz_houses1
set street_prefix = 'пер.'
WHERE street = 'Спортивный' AND id_region = 2;

UPDATE _not_privatiz_houses1
set street_prefix = 'пер.'
WHERE street = 'Заярский' AND id_region = 5;

UPDATE _not_privatiz_houses1
set street_prefix = 'пер.'
WHERE street = 'Рудничный' AND id_region = 5;

UPDATE _not_privatiz_houses1
set street_prefix = 'ул.'
WHERE street IS NOT NULL AND street <> '' AND street_prefix IS NULL;

UPDATE _not_privatiz_houses1
set street_prefix = 'ул.'
WHERE street_prefix REGEXP '^ул$';

UPDATE _not_privatiz_houses1
set street_prefix = 'туп.'
WHERE street_prefix = 'тупик/ ул.' AND street = 'Крылатый' AND id_region = 2;

UPDATE _not_privatiz_houses1
set street_prefix = 'пер.'
WHERE street = 'Чкалова' AND id_region = 7;

-- Обновляем номера домов
UPDATE _not_privatiz_houses1
set house = LOWER(house);

-- Правим вручную вот этот фейспалм, перенося метку "новостройка" в описание:
SELECT *
FROM registry._not_privatiz_houses1
WHERE house LIKE '%новострой%';

UPDATE _not_privatiz_houses1
set house = TRIM(house);

-- Проверяем, чтобы в таблице квартир не было адресов, которых нет в таблице зданий
SELECT *
FROM _not_privatiz_premises3 npp 
WHERE CONCAT(street_prefix,' ', CONCAT(street, ' ', house)) NOT IN
  (SELECT CONCAT(street_prefix,' ', CONCAT(street, ' ', house))
   FROM _not_privatiz_houses1);
   
-- Делаем подмену адресов в таблице квартир на идентификаторы
-- id_building = 357 и 406 - один и тот же дом (общежитие). Ошибка в их файле Excel
-- id_building = 7 и 22 - один и тот же дом. Ошибка в их файле Excel
UPDATE
  _not_privatiz_premises3 npp
set npp.id_building = (SELECT nph.id_record FROM _not_privatiz_houses1 nph
                        WHERE npp.street_prefix = nph.street_prefix AND
                              npp.street = nph.street AND
                              npp.house = nph.house AND
                              npp.id_region = nph.id_region
							  -- список зданий-призраков (2 из них дублируются (см. выше), остальные снесены и на их место
							  -- поставлены новостройки с тем же номером, а запись из Excel-файла не удалена о старом
                              AND nph.id_record NOT IN (2099,3008,2164,2179,2183,357,7,566,909,568) 
                              AND nph.house IS NOT NULL);

SELECT street_prefix, street, house, id_region
FROM _not_privatiz_houses1
WHERE id_record NOT IN (2099,3008,2164,2179,2183,357,7,566,909,568) AND house IS NOT NULL
GROUP BY street_prefix, street, house, id_region;

-- Сопоставляем адреса в kladr и таблице зданий и переносим здания на новый уровень обработки
INSERT INTO _not_privatiz_houses2
  (
    id_record,
    id_street,
    is_emergency,
    is_demolished,
    total_flats,
    mnpa_exclude,
    after_exclude_premises_nums,
    before_2013,
    I_2013,
    II_2013,
    III_2013,
    IV_2013,
    I_2014,
    II_2014,
    III_2014,
    `rest, date, num exclude MS`,
    `rest, date, num include MS`,
    privatiz_premises_not_kumi,
    privatiz_premises_kumi,
    DESCription
  )
SELECT a.id_record, id_street, a.is_emergency, a.is_demolished, a.total_flats,
  a.mnpa_exclude, a.after_exclude_premises_nums, a.before_2013, a.I_2013,
                                                                a.II_2013,
                                                                a.III_2013,
                                                                a.IV_2013,
                                                                a.I_2014,
                                                                a.II_2014,
                                                                a.III_2014,
  a.`rest, date, num exclude MS`,
  a.`rest, date, num include MS`,
  a.privatiz_premises_not_kumi,
  a.privatiz_premises_kumi,
  a.description
FROM (
SELECT *,
  CASE id_region 
    WHEN 1 THEN 'жилрайон. Бикей' 
    WHEN 2 THEN 'жилрайон. Центральный' 
    WHEN 3 THEN 'жилрайон. Энергетик' 
    WHEN 4 THEN 'жилрайон. Гидростроитель' 
    WHEN 5 THEN 'жилрайон. Осиновка' 
    WHEN 6 THEN 'жилрайон. Падун' 
    WHEN 7 THEN 'жилрайон. Порожский' 
    WHEN 8 THEN 'жилрайон. Сухой' 
    ELSE id_region END AS region
FROM _not_privatiz_houses1 npp) a INNER JOIN v_kladr_streets ON
  CONCAT(a.region,', ',CONCAT(a.street_prefix,' ', a.street)) = v_kladr_streets.street_name;
  
-- Обрабатываем собственную тупость (не перенес номера домов)
UPDATE registry._not_privatiz_houses2
set _not_privatiz_houses2.house = (SELECT registry._not_privatiz_houses1.house
                                    FROM registry._not_privatiz_houses1 
                                    WHERE _not_privatiz_houses1.id_record = _not_privatiz_houses2.id_record)
  
-- переносим квартиры на новый уровень обработки (4й)
INSERT INTO _not_privatiz_premises4(
  id_record,
  id_building,
  house_premises_count,
  crash_bit,
  flat_number,
  total_area,
  total_flat_count,
  description,
  id_fund_type
  )
SELECT npp.id_record, npp.id_building, npp.house_premises_count, npp.crash_bit, npp.flat_number, npp.total_area, npp.total_flat_count, npp.description, npp.id_fund_type
FROM _not_privatiz_premises3 npp

-- удаляем дубликаты квартир, предварительно проверив, что это дубликаты комнат

-- Правим поле is_emergency
UPDATE _not_privatiz_houses2
set is_emergency = NULL
WHERE TRIM(is_emergency) = '';

UPDATE _not_privatiz_houses2
set description = CONCAT(description,' || ',is_emergency), is_emergency = NULL
WHERE is_emergency IS NOT NULL AND is_emergency != 'а';

-- Правим поле mnpa_exclude
UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,'/ ','/')
WHERE mnpa_exclude REGEXP '^(П|Р)[0-9]+/ [0-9][0-9].[0-9][0-9].[0-9][0-9]$';

UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,' /','/')
WHERE mnpa_exclude REGEXP '^(П|Р)[0-9]+ /[0-9][0-9].[0-9][0-9].[0-9][0-9]$';

UPDATE _not_privatiz_houses2
SET mnpa_exclude = REPLACE(mnpa_exclude, '№','')
WHERE mnpa_exclude NOT REGEXP '^(П|Р)[0-9]+/[0-9][0-9].[0-9][0-9].[0-9][0-9]$';

UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,'/ ','/')
WHERE mnpa_exclude NOT REGEXP '^(П|Р)[0-9]+/[0-9][0-9].[0-9][0-9].[0-9][0-9]$';

UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,' /','/')
WHERE mnpa_exclude NOT REGEXP '^(П|Р)[0-9]+/[0-9][0-9].[0-9][0-9].[0-9][0-9]$';

UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,', ',' ')
WHERE mnpa_exclude 
  REGEXP '^(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9][,; ][ ]*(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9]$';
  
UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,'; ',' ')
WHERE mnpa_exclude 
  REGEXP '^(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9][,; ][ ]*(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9]$';
  
UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,';',' ')
WHERE mnpa_exclude 
  REGEXP '^(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9][,; ][ ]*(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9]$';
  
UPDATE _not_privatiz_houses2
set mnpa_exclude = REPLACE(mnpa_exclude,'  ',' ')
WHERE mnpa_exclude 
  REGEXP '^(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9][,; ][ ]*(П|Р)[0-9]+/[0-9][0-9][.][0-9][0-9][.][0-9][0-9]$';
  
UPDATE _not_privatiz_houses2 nph
SET mnpa_exclude = NULL
WHERE 
   mnpa_exclude = '';
   
-- Очередная процедура с курсором для нормализации и вставки реквизитов в таблицу _not_privatiz_buildings_restrictions
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.normalize_buldings_restrictions()
BEGIN
  -- Объявляем колонки таблицы
Declare id_building INT;
DECLARE restriction varchar(255);
DECLARE restriction_current varchar(255);
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT id_record, mnpa_exclude
      FROM _not_privatiz_houses2 nph
      WHERE nph.mnpa_exclude 
        REGEXP '^(П|Р)[0-9]+/[0-9][0-9].[0-9][0-9].[0-9][0-9][+]?([ ](П|Р)[0-9]+/[0-9][0-9].[0-9][0-9].[0-9][0-9][+]?)*$';
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_building,
    restriction;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE restriction <> '' DO
    set restriction_current = 
      SUBSTRING(restriction, 1, LOCATE(' ',restriction) - 1);
    IF (restriction_current = '') THEN
      set restriction_current = restriction;
      set restriction = '';
    ELSE 
      set restriction = 
        SUBSTRING(restriction, LOCATE(' ',restriction) + 1);
    END IF;
    INSERT INTO _not_privatiz_buildings_restrictions
      (id_building, restriction_draft)
    VALUES (id_building, restriction_current);
  END WHILE;
END WHILE;
CLOSE cursor1;
COMMIT;
END
   
UPDATE _not_privatiz_buildings_restrictions
set number = SUBSTRING(restriction_draft, 1, LOCATE('/', restriction_draft) - 1),
  `date` = STR_TO_DATE(
    TRIM('+' FROM SUBSTRING(restriction_draft, LOCATE('/', restriction_draft) + 1)),'%d.%m.%Y'
  ),
  id_restriction_type = IF(LOCATE('+', restriction_draft) > 0, 1, 2);
  
UPDATE registry._not_privatiz_houses2
  set is_emergency = TRIM(is_emergency);
  
-- Корректируем поля is_emergency, is_demolished
UPDATE registry._not_privatiz_houses2
  set is_emergency = IF(is_emergency = 'а',1,0);
  
UPDATE _not_privatiz_houses2
set is_demolished = NULL
WHERE TRIM(is_demolished) = '';

-- Вставка ограничений в таблицу _not_privatiz_buildings_ownerships
INSERT INTO registry._not_privatiz_buildings_ownerships
  (
  id_building,
  description,
  `date`,
  id_ownership_type
  )
SELECT id_record, is_demolished, NOW(), 1
FROM _not_privatiz_houses2 h
WHERE is_demolished IS NOT NULL AND is_demolished <> 'общ.'
UNION ALL
SELECT id_record, NULL, NOW(), 2
FROM _not_privatiz_houses2
WHERE is_emergency = 1;

-- Правим crash_bit в таблице квартир
UPDATE _not_privatiz_premises4
set crash_bit = NULL
WHERE crash_bit = '';

INSERT INTO _not_privatiz_premises_ownerships(
  id_premises,
  `date`,
  description,
  id_ownership_type
  )
SELECT id_record, NOW(), NULL, IF(crash_bit = 'а', 2, 1)
FROM registry._not_privatiz_premises4
WHERE crash_bit IS NOT NULL;

UPDATE registry._not_privatiz_premises4
set total_flat_count = 0
WHERE total_flat_count = '';

-- Задаем базовые состояния для помещений (не достоверные)
UPDATE _not_privatiz_premises4
set id_state = 1
WHERE total_flat_count = '0';

UPDATE _not_privatiz_premises4
set id_state = 5
  WHERE total_flat_count <> '0';
  
-- Обрабатываем таблицу общежитий
UPDATE _premises_of_hostels1
set `Примечание` = CONCAT(`Примечание`,' || ', Column2)
WHERE Column2 IS NOT NULL AND Column2 <> '';

UPDATE registry._premises_of_hostels1_1
set `№ Этажа` = NULL
WHERE `№ Этажа` = '';

-- Переносим общаги на этап 2
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.reorganize_premises_of_hostels()
BEGIN
  -- Объявляем колонки таблицы
Declare street VARCHAR(255);
Declare house VARCHAR(255);
Declare floor_ VARCHAR(255);
Declare srn VARCHAR(255);
Declare premise_number VARCHAR(255);
Declare total_area VARCHAR(255);
Declare living_area VARCHAR(255);
Declare premises_count VARCHAR(255);
Declare description VARCHAR(255);
Declare commercial_count VARCHAR(255);
Declare special_count VARCHAR(255);
Declare is_commercial VARCHAR(255);
Declare is_special VARCHAR(255);
DECLARE evaluated bool DEFAULT FALSE;
  -- Объявляем вспомогательные буферы
Declare srn_buff VARCHAR(255) DEFAULT '';
Declare street_buff VARCHAR(255) DEFAULT '';
Declare house_buff VARCHAR(255) DEFAULT '';
Declare floor_buff VARCHAR(255) DEFAULT '';
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT 
        `Улица/ Sобщ ж/п в доме`,
        `№ дома`,
        `№ Этажа`,
        `№ ж/п`,
        `Общая площадь, м2`,
        `Жилая площадь, м2`,
        `Количество помещений`,
        Примечание,
        `отнес к коммерч. Жф, (к)`,
        `Итого  жп в комм фонде`,
        `отнес к спец.жф, (с)`,
        Column1
      FROM _premises_of_hostels1_1 npp
      WHERE (`№ п/п` IS NULL OR `№ п/п` <> 'Всего по зданию: Sобщ,  м2') AND 
            (`№ Этажа` IS NULL OR `№ Этажа` <> 'Итого по этажу');
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION;
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    street,
    house,
    floor_,
    premise_number,
    total_area,
    living_area,
    premises_count, 
    description,
    is_commercial,
    commercial_count,
    is_special,
    special_count;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  -- TODO
  SET evaluated = FALSE;
  -- Если это строка здания
  IF (street IS NOT NULL AND TRIM(street) <> '') THEN
    IF (house IS NOT NULL AND TRIM(house) <> '') THEN
      set house_buff = TRIM(house);
    END IF;
    SET street_buff = TRIM(street);
    SET evaluated = TRUE;
  ELSEIF (house IS NOT NULL AND TRIM(house) <> '') THEN
    set srn_buff = TRIM(srn);
  END IF;
  IF (floor_ IS NOT NULL AND TRIM(floor_) <> '') THEN
    SET floor_buff = TRIM(floor_);
  END IF;

  IF ((premise_number IS NOT NULL) AND (TRIM(premise_number) <> '')) THEN
    IF ((street_buff IS NULL) OR (TRIM(street_buff) = '') OR
      (house_buff IS NULL) OR (TRIM(house_buff) = '') OR
      (floor_buff IS NULL) OR (TRIM(floor_buff) = '')) THEN
      SIGNAL SQLSTATE 'ERR0R' SET MESSAGE_TEXT = street_buff;
    END IF;
    INSERT INTO _premises_of_hostels2
    VALUES(
      TRIM(street_buff),
      TRIM(house_buff),
      TRIM(floor_buff),
      TRIM(premise_number),
      TRIM(total_area),
      TRIM(living_area),
      TRIM(premises_count),
      TRIM(description),
      TRIM(is_commercial),
      TRIM(commercial_count),
      TRIM(is_special),
      TRIM(special_count));
    SET evaluated = TRUE;
  END IF;
  IF (evaluated = false) THEN
    SIGNAL SQLSTATE 'ERR0R' SET MESSAGE_TEXT = street_buff;
  END IF;
END WHILE;
CLOSE cursor1;
-- Проверяем результат и откатываемся
SELECT *
FROM _not_privatiz_premises2 npp;
COMMIT;
END

-- Добавляем id_record как primary_key для общаг

-- Проверяем адреса общаг и делаем привязки
UPDATE registry._premises_of_hostels2
set id_building = (
SELECT nph.id_record
FROM _not_privatiz_houses2 nph INNER JOIN
(SELECT id_record, house, v_kladr_streets.id_street, UPPER(CONCAT('жилрайон. Центральный, ул. ',street))
FROM registry._premises_of_hostels2 INNER JOIN registry.v_kladr_streets
  ON UPPER(CONCAT('жилрайон. Центральный, ул. ',_premises_of_hostels2.street)) = 
  UPPER(registry.v_kladr_streets.street_name)
UNION ALL
SELECT id_record, house, v_kladr_streets.id_street, UPPER(CONCAT('жилрайон. Гидростроитель, ул. ',street))
FROM registry._premises_of_hostels2 INNER JOIN registry.v_kladr_streets
  ON UPPER(CONCAT('жилрайон. Гидростроитель, ул. ',_premises_of_hostels2.street)) = 
  UPPER(registry.v_kladr_streets.street_name)
UNION ALL
SELECT id_record, house, v_kladr_streets.id_street, UPPER(CONCAT('жилрайон. Энергетик, ул. ',street))
FROM registry._premises_of_hostels2 INNER JOIN registry.v_kladr_streets
  ON UPPER(CONCAT('жилрайон. Энергетик, ул. ',_premises_of_hostels2.street)) = 
  UPPER(registry.v_kladr_streets.street_name)) a ON a.id_street = nph.id_street AND
  a.house = nph.house AND nph.id_record NOT IN (2099,3008,2164,2179,2183,357,7,566,909,568)
WHERE a.id_record = registry._premises_of_hostels2.id_record)

-- Нормализуем номера квартир в общагах
UPDATE _premises_of_hostels2
set premise_number = LOWER(premise_number);

-- Выборка невалидных номеров квартир (правка вручную)
SELECT *
FROM _premises_of_hostels2 poh
WHERE premise_number NOT REGEXP '^[0-9]*[аб]*$' AND
 premise_number NOT REGEXP '^[0-9]*[аб]*[(][0-9]*([,][0-9])*[)]$';
 
UPDATE _premises_of_hostels2
SET
  flat_prepair = SUBSTRING(premise_number, 1, LOCATE('(',premise_number) - 1),
  rooms =SUBSTRING(premise_number, LOCATE('(',premise_number) + 1, 
  LOCATE(')',premise_number) - LOCATE('(',premise_number) - 1)
WHERE premise_number REGEXP '^[0-9]*[аб]*[(][0-9]*[абвг]*([,][0-9]*[абвг]*)*[)]$';

--Переносим непроблемные квартиры в бывших общагах на следующий шаг
INSERT INTO
_premises_of_hostels3
SELECT poh.id_record, id_building, poh.floor, poh.premise_number,
  poh.total_area, poh.living_area, poh.premises_count, poh.description,
  poh.is_commercial, poh.commercial_count, poh.is_special, poh.special_count
FROM _premises_of_hostels2 poh
WHERE premise_number NOT REGEXP '^[0-9]*[аб]*$' AND
flat_prepair IS NULL;

INSERT INTO
_premises_of_hostels3
SELECT poh.id_record, id_building, poh.floor, poh.premise_number,
  poh.total_area, poh.living_area, poh.premises_count, poh.description,
  poh.is_commercial, poh.commercial_count, poh.is_special, poh.special_count
FROM _premises_of_hostels2 poh
WHERE premise_number REGEXP '^[0-9]*[аб]*$';

INSERT INTO
_premises_of_hostels3
SELECT poh.id_record, id_building, poh.floor, poh.flat_prepair,
  poh.total_area, poh.living_area, poh.premises_count, poh.description,
  poh.is_commercial, poh.commercial_count, poh.is_special, poh.special_count
FROM _premises_of_hostels2 poh
WHERE premise_number NOT REGEXP '^[0-9]*[аб]*$' AND
flat_prepair IS NOT NULL;

-- Ищем квартиры, расположенные в нескольких строках таблицы
SELECT *
FROM _premises_of_hostels2 npp
WHERE flat_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _premises_of_hostels2 npp2
   WHERE npp.id_building = npp2.id_building AND
    npp.flat_prepair = npp2.flat_prepair) > 1;
	
-- Вставляем непроблемные квартиры
INSERT INTO _rooms_of_hostels1 (id_premises, sub_premises_number, total_area, description, is_commercial, commercial_count,
  is_special, special_count)
SELECT npp.id_record, rooms, npp.total_area, npp.description, npp.is_commercial, npp.commercial_count, npp.is_special, npp.special_count
FROM _premises_of_hostels2 npp
WHERE flat_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _premises_of_hostels2 npp2
   WHERE npp.id_building = npp2.id_building AND
    npp.flat_prepair = npp2.flat_prepair) = 1;
	
-- Вставляем проблемные квартиры
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.reorganize_problem_rooms_hostels()
BEGIN
 -- Объявляем колонки таблицы
Declare id_record INT;
Declare id_building INT;
Declare flat VARCHAR(255);
Declare room_number VARCHAR(255);
Declare total_area VARCHAR(255);
Declare description VARCHAR(255);
Declare is_commercial VARCHAR(255);
Declare commercial_count VARCHAR(255);
Declare is_special varchar(255);
Declare special_count VARCHAR(255);
  -- Объявляем вспомогательные буферы
Declare id_record_buff INT DEFAULT 0;
Declare id_building_buff VARCHAR(255) DEFAULT '';
Declare flat_buff VARCHAR(255) DEFAULT '';
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT npp.id_record, npp.id_building, npp.flat_prepair, rooms, npp.total_area, npp.description, npp.is_commercial, npp.commercial_count, npp.is_special, npp.special_count
FROM _premises_of_hostels2 npp
WHERE flat_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _premises_of_hostels2 npp2
   WHERE npp.id_building = npp2.id_building AND
    npp.flat_prepair = npp2.flat_prepair) > 1;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_record,
    id_building,
    flat,
    room_number,
    total_area,
    description,
    is_commercial, commercial_count,
    is_special, special_count;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  IF ((TRIM(id_building) <> id_building_buff) || 
    (TRIM(flat) <> flat_buff)) THEN
      SET id_building_buff = TRIM(id_building);
      SET id_record_buff = TRIM(id_record);
      SET flat_buff = TRIM(flat);
  END IF;
  
  IF ((id_building_buff IS NULL) OR (TRIM(id_building_buff) = '') OR
      (flat_buff IS NULL) OR (TRIM(flat_buff) = '') OR
      (id_record_buff IS NULL) OR (TRIM(id_record_buff) = '')) THEN
    SIGNAL SQLSTATE 'ERR0R' SET MESSAGE_TEXT = id_building_buff;
  ELSE
    INSERT INTO _rooms_of_hostels1 (id_premises, sub_premises_number, total_area, 
      description, is_commercial, commercial_count, is_special, special_count)
    VALUES(id_record_buff,
      TRIM(room_number),
      TRIM(total_area),
      TRIM(description),
      TRIM(is_commercial),
      TRIM(commercial_count),
      TRIM(is_special),
      TRIM(special_count));
  END IF;
END WHILE;
CLOSE cursor1;
COMMIT;
END

-- ул. Заярская 16 - то ли 2 дома с одним номером, то ли я хз, необходимо ручное расщипление адресов

-- Удаляем дубликаты квартир из общаг
DELETE FROM _premises_of_hostels3
WHERE id_record IN
(SELECT a.id_record
FROM (
SELECT npp.id_record, npp.id_building, npp.flat_prepair, rooms, npp.total_area, npp.description, npp.is_commercial, npp.commercial_count, npp.is_special, npp.special_count
FROM _premises_of_hostels2 npp
WHERE flat_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _premises_of_hostels2 npp2
   WHERE npp.id_building = npp2.id_building AND
    npp.flat_prepair = npp2.flat_prepair) > 1) a
WHERE a.id_record NOT IN (SELECT id_premises FROM registry._rooms_of_hostels1));

-- Расщепляем квартиры общаг
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.reorganize_problem_rooms_hostels()
BEGIN
 -- Объявляем колонки таблицы
Declare id_record INT;
Declare id_building INT;
Declare flat VARCHAR(255);
Declare room_number VARCHAR(255);
Declare total_area VARCHAR(255);
Declare description VARCHAR(255);
Declare is_commercial VARCHAR(255);
Declare commercial_count VARCHAR(255);
Declare is_special varchar(255);
Declare special_count VARCHAR(255);
  -- Объявляем вспомогательные буферы
Declare id_record_buff INT DEFAULT 0;
Declare id_building_buff VARCHAR(255) DEFAULT '';
Declare flat_buff VARCHAR(255) DEFAULT '';
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT npp.id_record, npp.id_building, npp.flat_prepair, rooms, npp.total_area, npp.description, npp.is_commercial, npp.commercial_count, npp.is_special, npp.special_count
FROM _premises_of_hostels2 npp
WHERE flat_prepair IS NOT NULL AND
  (SELECT COUNT(*) FROM _premises_of_hostels2 npp2
   WHERE npp.id_building = npp2.id_building AND
    npp.flat_prepair = npp2.flat_prepair) > 1;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_record,
    id_building,
    flat,
    room_number,
    total_area,
    description,
    is_commercial, commercial_count,
    is_special, special_count;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  IF ((TRIM(id_building) <> id_building_buff) || 
    (TRIM(flat) <> flat_buff)) THEN
      SET id_building_buff = TRIM(id_building);
      SET id_record_buff = TRIM(id_record);
      SET flat_buff = TRIM(flat);
  END IF;
  
  IF ((id_building_buff IS NULL) OR (TRIM(id_building_buff) = '') OR
      (flat_buff IS NULL) OR (TRIM(flat_buff) = '') OR
      (id_record_buff IS NULL) OR (TRIM(id_record_buff) = '')) THEN
    SIGNAL SQLSTATE 'ERR0R' SET MESSAGE_TEXT = id_building_buff;
  ELSE
    INSERT INTO _rooms_of_hostels1 (id_premises, sub_premises_number, total_area, 
      description, is_commercial, commercial_count, is_special, special_count)
    VALUES(id_record_buff,
      TRIM(room_number),
      TRIM(total_area),
      TRIM(description),
      TRIM(is_commercial),
      TRIM(commercial_count),
      TRIM(is_special),
      TRIM(special_count));
  END IF;
END WHILE;
CLOSE cursor1;
COMMIT;
END

INSERT INTO _rooms_of_hostels2(id_premises, sub_premises_number, total_area, description, is_commercial, commercial_count,
  is_special, special_count)
SELECT id_premises, sub_premises_number, total_area, description, is_commercial, commercial_count,
  is_special, special_count
FROM _rooms_of_hostels1
WHERE sub_premises_number NOT LIKE '%,%';

-- Корректируем дублирование площади комнат ручками, ибо скрипт импорта кривой
SELECT *
FROM registry._rooms_of_hostels2 a
WHERE id_premises IN (SELECT id_premises FROM
registry._rooms_of_hostels2 b WHERE
  (SELECT COUNT(*) FROM registry._rooms_of_hostels2 c
    WHERE b.id_premises = c.id_premises
    GROUP BY id_premises) > 1);
	
-- Корректируем is_special и добавляем специализированные комнаты
UPDATE _premises_of_hostels2
set special_count = NULL
WHERE special_count = '';

UPDATE _premises_of_hostels2
set special_count = NULL,
  is_special = NULL
WHERE special_count = '0';

UPDATE _premises_of_hostels2
set is_special = LOWER(is_special)
WHERE special_count IS NOT NULL AND special_count <> 1;

CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.adding_new_hostels_rooms()
BEGIN
  -- Объявляем колонки таблицы
Declare id_premises INT;
Declare sub_premises_number_list VARCHAR(255);
Declare sub_premises_number_current VARCHAR(255);
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT id_record, SUBSTRING(is_special,3, LOCATE(')', is_special) - 3) FROM _premises_of_hostels2
      WHERE special_count IS NOT NULL AND special_count <> 1 AND rooms IS NULL;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_premises,
    sub_premises_number_list;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE sub_premises_number_list <> '' DO
    set sub_premises_number_current = 
      SUBSTRING(sub_premises_number_list, 1, LOCATE(',',sub_premises_number_list) - 1);
    IF (sub_premises_number_current = '') THEN
      set sub_premises_number_current = sub_premises_number_list;
      set sub_premises_number_list = '';
    ELSE 
      set sub_premises_number_list = 
        SUBSTRING(sub_premises_number_list, LOCATE(',',sub_premises_number_list) + 1);
    END IF;
    INSERT INTO _rooms_of_hostels2(id_premises, sub_premises_number, total_area, description,
      is_commercial, commercial_count, is_special, special_count)
    VALUES (id_premises, sub_premises_number_current, 0, NULL, NULL,
      NULL, 'с', 1);
  END WHILE;
END WHILE;
CLOSE cursor1;
COMMIT;
END

UPDATE _rooms_of_hostels2
set is_commercial = NULL,
  commercial_count = NULL
WHERE is_commercial = '';

UPDATE _rooms_of_hostels2
set is_special = NULL,
  special_count = NULL
WHERE is_special = '';

INSERT INTO _rooms_of_hostels3
SELECT id_room, id_premises, sub_premises_number, total_area, description, 
  IF (is_commercial IS NOT NULL, 2, IF (is_special IS NOT NULL, 3, NULL))
FROM _rooms_of_hostels2;

UPDATE _premises_of_hostels3
set is_special = NULL
WHERE is_special = '';

UPDATE _premises_of_hostels3
set is_special = LOWER(is_special)
WHERE is_special IS NOT NULL AND is_special <> 'с' AND special_count = 1;

UPDATE _premises_of_hostels2
set is_special = (SELECT _premises_of_hostels3.is_special FROM _premises_of_hostels3 WHERE _premises_of_hostels2.id_record =
  _premises_of_hostels3.id_record)
WHERE is_special IS NOT NULL AND is_special <> 'с' AND special_count = 1;

-- Валидируем помещения социального найма
SELECT *
FROM _premises_of_hostels2
WHERE is_special IS NOT NULL AND is_special <> 'с' AND 
  is_special REGEXP 'с[(][0-9]*,[0-9]*,[0-9]*,[0-9]*[)]' AND
  (SELECT COUNT(*) FROM _rooms_of_hostels3 
    WHERE _rooms_of_hostels3.id_premises = _premises_of_hostels2.id_record AND _rooms_of_hostels3.id_fund_type = 3 IS NOT NULL
    GROUP BY _rooms_of_hostels3.id_premises) <> 4
	
SELECT *
FROM _premises_of_hostels2
WHERE is_special IS NOT NULL AND is_special <> 'с' AND 
  is_special REGEXP 'с[(][0-9]*,[0-9]*,[0-9]*[)]' AND
  (SELECT COUNT(*) FROM _rooms_of_hostels3 
    WHERE _rooms_of_hostels3.id_premises = _premises_of_hostels2.id_record AND _rooms_of_hostels3.id_fund_type = 3 IS NOT NULL
    GROUP BY _rooms_of_hostels3.id_premises) <> 3
	
SELECT *
FROM _premises_of_hostels2
WHERE is_special IS NOT NULL AND is_special <> 'с' AND 
  is_special REGEXP 'с[(][0-9]*,[0-9]*[)]' AND
  (SELECT COUNT(*) FROM _rooms_of_hostels3 
    WHERE _rooms_of_hostels3.id_premises = _premises_of_hostels2.id_record AND _rooms_of_hostels3.id_fund_type = 3 IS NOT NULL
    GROUP BY _rooms_of_hostels3.id_premises) <> 2
	
SELECT *
FROM _premises_of_hostels2
WHERE is_special IS NOT NULL AND is_special <> 'с' AND 
  is_special REGEXP 'с[(][0-9]*[)]' AND
  (SELECT COUNT(*) FROM _rooms_of_hostels3 
    WHERE _rooms_of_hostels3.id_premises = _premises_of_hostels2.id_record AND _rooms_of_hostels3.id_fund_type = 3 IS NOT NULL
    GROUP BY _rooms_of_hostels3.id_premises) <> 1

UPDATE _premises_of_hostels3
set is_commercial = NULL,
  commercial_count = NULL
WHERE is_commercial = '';

-- Переносим по глупости удаленную колонку premises_count
UPDATE _premises_of_hostels4 
set premises_count = (
  SELECT premises_count 
    FROM _premises_of_hostels3
    WHERE _premises_of_hostels3.id_record = _premises_of_hostels4.id_record);

-- Неоднозначность по фонду квартир 285, 267, 266, 251
-- убираем маркер принадлежности к спец. фонду,  где написано, что конкретные комнаты к нему относятся
UPDATE _premises_of_hostels3
set is_special = NULL,
  special_count = NULL
WHERE is_special IS NOT NULL AND is_special <> 'с';

-- Правим коммерческий жилой фонд
---------------------------------
UPDATE _commercial_fund2
set `Адрес, г. Братск, ул. (б, пер)` = '20 Партсъезда'
WHERE `Адрес, г. Братск, ул. (б, пер)` = 'XX Партсъезда';

-- Добавляем поле префикса в  _commercial_fund2 и устанавлеваем ему 
-- дефолтное значение "ул."
-- правим НЕ улицы
UPDATE _commercial_fund2
set id_street_prefix = 'б-р.' 
WHERE `Адрес, г. Братск, ул. (б, пер)` = 'Космонавтов' AND `ж. р.` = 'ц';

UPDATE _commercial_fund2
set id_street_prefix = 'пер.',
  `Адрес, г. Братск, ул. (б, пер)` = 'Лазурный'
WHERE `Адрес, г. Братск, ул. (б, пер)` = 'Лазурный, пер.' AND `ж. р.` = 'пад';

UPDATE _commercial_fund2
set id_street_prefix = 'пер.',
  `Адрес, г. Братск, ул. (б, пер)` = 'Киевский'
WHERE `Адрес, г. Братск, ул. (б, пер)` = 'Киевский, пер.' AND `ж. р.` = 'пор';

UPDATE _commercial_fund2
set id_street_prefix = 'пер.',
  `Адрес, г. Братск, ул. (б, пер)` = 'Лунный'
WHERE `Адрес, г. Братск, ул. (б, пер)` = 'Лунный, пер.' AND `ж. р.` = 'с';

-- Остальные правим ручками

-- Проверям, чтобы запрос не возвращал ничего
SELECT *, CONCAT(region,', ',CONCAT(id_street_prefix,' ',address)) AS kladr_address
FROM (
  SELECT `Адрес, г. Братск, ул. (б, пер)` AS address, id_street_prefix,
    CASE `ж. р.` 
    WHEN 'ч' THEN 'жилрайон. Чекановский'
    WHEN 'ц' THEN 'жилрайон. Центральный'
    WHEN 'о' THEN 'жилрайон. Осиновка'
    WHEN 'пор' THEN 'жилрайон. Порожский'
    WHEN 'пад' THEN 'жилрайон. Падун'
    WHEN 'э' THEN 'жилрайон. Энергетик'
    WHEN 'г' THEN 'жилрайон. Гидростроитель'
    WHEN 'с' THEN 'жилрайон. Сухой'
    WHEN 'б' THEN 'жилрайон. Бикей'
    END AS region
FROM _commercial_fund2) a
WHERE CONCAT(region,', ',CONCAT(id_street_prefix,' ',address)) NOT IN (
  SELECT street_name FROM v_kladr_streets vks
  );
  
-- Делаем привязку к кладеру
UPDATE registry._commercial_fund2
set id_street = 
(SELECT id_street
FROM (
  SELECT `№ п/п`,`Адрес, г. Братск, ул. (б, пер)` AS address, id_street_prefix, `№ дома`,
    CASE `ж. р.` 
    WHEN 'ч' THEN 'жилрайон. Чекановский'
    WHEN 'ц' THEN 'жилрайон. Центральный'
    WHEN 'о' THEN 'жилрайон. Осиновка'
    WHEN 'пор' THEN 'жилрайон. Порожский'
    WHEN 'пад' THEN 'жилрайон. Падун'
    WHEN 'э' THEN 'жилрайон. Энергетик'
    WHEN 'г' THEN 'жилрайон. Гидростроитель'
    WHEN 'с' THEN 'жилрайон. Сухой'
    WHEN 'б' THEN 'жилрайон. Бикей'
    END AS region, `ж. р.`
FROM _commercial_fund2) a INNER JOIN v_kladr_streets ON
CONCAT(region,', ',CONCAT(id_street_prefix,' ',address)) = v_kladr_streets.street_name
WHERE a.address = _commercial_fund2.`Адрес, г. Братск, ул. (б, пер)` AND
  a.`№ дома` = _commercial_fund2.`№ дома` AND a.`ж. р.` = _commercial_fund2.`ж. р.` AND
  a.`№ п/п` = _commercial_fund2.`№ п/п`);

-- Делаем ручные правки непривязавшихся адресов
  
-- Делаем привязку к зданию и перенос на следующий уровень обработки
UPDATE _commercial_fund2
set `№ дома` = LOWER(`№ дома`);

-- Импортируем дома из Чекановского, т.к. на них завязка в коммерческом фонде
-- Валидируем адреса по кладеру
SELECT *, CONCAT('жилрайон. Чекановский, ',`Улица,переулок, проезд`, CONCAT(' ', `Наименование улицы`))
FROM registry._not_privatiz_houses_chekanovsk1
WHERE CONCAT('жилрайон. Чекановский, ',`Улица,переулок, проезд`, CONCAT(' ', `Наименование улицы`))
  NOT IN (SELECT street_name FROM v_kladr_streets vks);
  
INSERT INTO _not_privatiz_houses_chekanovsk2
SELECT b.id_street, a.`№ дома`, a.`Аварийный ж/д`, a.`Жилой дом снесен`,
  a.`Общее количество ж/п в доме по данным БТИ`,
  a.`№ кв. приватизированных и  непрошедших через КУМИ по данным`,
  a.`S общ. ж/п по дому, м2`,
  a.`Итого муниципальных ж/п (номера кв)`,
  a.`Кол-во ЖП`,
  a.Примечание,
  a.`№ жп отнес к коммерч. Жф`,
  a.`Кол-во ЖП1`,
  a.`№ жп отнес к спец.жф`
FROM registry._not_privatiz_houses_chekanovsk1 a INNER JOIN v_kladr_streets b ON
  CONCAT('жилрайон. Чекановский, ',`Улица,переулок, проезд`, CONCAT(' ', `Наименование улицы`))
  = b.street_name;
  
 -- 108 домов из Чекановского добавил в таблицу зданий (этап 2)
 INSERT INTO _not_privatiz_houses2(
	id_street,
	house,
	is_emergency,
	is_demolished,
	total_flats,
	mnpa_exclude,
	after_exclude_premises_nums,
	before_2013,
	I_2013,
	II_2013,
	III_2013,
	IV_2013,
	I_2014,
	II_2014,
	III_2014,
	`rest, date, num exclude MS`,
	`rest, date, num include MS`,
	privatiz_premises_not_kumi,
	privatiz_premises_kumi,
	description
  )
SELECT id_street, house, is_emergency, is_demolished,
  total_flats, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
  NULL, NULL, NULL, NULL, Примечание
FROM _not_privatiz_houses_chekanovsk2;

-- добавляем ограничения "снесено" на добавленные дома в таблицу ограничений
INSERT INTO _not_privatiz_buildings_ownerships
  (
id_building,
  `date`,
  description,
  id_ownership_type
  )
SELECT id_record, NOW(), NULL, 1
FROM _not_privatiz_houses2 a INNER JOIN
  _not_privatiz_houses_chekanovsk2 b ON a.id_street = b.id_street AND
  a.house = b.house AND b.is_demolished IS NOT NULL;
  
 -- Привязываем реквизиты на включение/исключение из коммерческого фонда к идентификаторам зданий
INSERT INTO _commercial_fund3
SELECT `№ п/п`, `№ по постановлению`, b.id_record,
`№ кв.`, `Включен   (№, дата постановления (мэра, администрации)`,
`Искл из коммерческого фонда ( постановление от- №)`, Примечание,
`Аварийное (снесено)жилье, а/с`,
`ЖП в комм.фонде`,
`ЖП искл из комм.фонда`,`№ жилого помещения`,
`Площадь общая`,
Наниматель,
`Тип найма`,
`Договор №`,
`Срок действия`,
Прописка,
`Примечание Воробьево ЕЛ`,
`Примечание Тануйловой МА`,
`Примечание Лскавян АН`
FROM _commercial_fund2 a LEFT JOIN
_not_privatiz_houses2 b ON a.id_street = b.id_street AND
  a.`№ дома` = b.house;
  
SELECT *
FROM _commercial_fund3 LEFT JOIN (
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',n_post)  AS `description`,
TRIM(SUBSTRING(draft_num_date,1, LOCATE('от', draft_num_date) - 2)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date, LOCATE('от', draft_num_date) + 2)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('№', include_restriction) - 1) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('№', include_restriction) + 1)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) NOT REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') a
UNION ALL
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',n_post) AS `description`,
TRIM(SUBSTRING(draft_num_date, LOCATE('№', draft_num_date) + 1)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date,1, LOCATE('№', draft_num_date) - 1)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('от', include_restriction) - 2) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('от', include_restriction) + 2)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') b) a
  ON _commercial_fund3.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON _commercial_fund3.id_record = b.id_record;

-- корректируем номера квартир в коммерческом фонде
UPDATE _commercial_fund3
set premise_number = TRIM(premise_number);

-- 41 помещение, по которых засып на импорт реквизитов вкл. искл. коммерческого фонда
SELECT DISTINCT a.*
FROM registry._commercial_fund3 a
LEFT JOIN (SELECT id_record, b.id_building, b.flat_number FROM _not_privatiz_premises4 b
            UNION ALL
           SELECT id_record,  b.id_building, b.premise_number FROM _premises_of_hostels4 b) b ON
a.id_building = b.id_building AND a.premise_number = b.flat_number
WHERE b.id_record IS NULL AND
  a.premise_number NOT REGEXP '^[0-9]*$';
  
 -- Смещяем идентификаторы общежитий в таблицах с общагами и комнатами общаг на 14000
 -- Это нужно для слияния с таблицей квартир и комнат обычных зданий
 UPDATE _premises_of_hostels4 
set id_record = id_record+14000;

UPDATE _rooms_of_hostels3
set id_premises = id_premises+14000;

-- Корректируем площади для импорта в поле DOUBLE
UPDATE _premises_of_hostels4
set total_area = 0
WHERE total_area IS NULL;

UPDATE _premises_of_hostels4
set living_area = 0
WHERE living_area IS NULL;
-- Объединяем все квартиры в единую таблицу
INSERT INTO _premises_total
SELECT npp.id_record, npp.id_building, NULL, npp.house_premises_count, npp.crash_bit,
  npp.flat_number,CAST(REPLACE(npp.total_area,',','.') AS decimal(10,3)), 0, npp.total_flat_count, npp.description, npp.id_fund_type,
  npp.id_state
FROM _not_privatiz_premises4 npp
UNION ALL
SELECT poh.id_record, poh.id_building, poh.floor, premises_count, NULL,
  poh.premise_number, CAST(REPLACE(poh.total_area,',','.') AS decimal(10,3)), 
  CAST(REPLACE(poh.living_area,',','.') AS decimal(10,3)), poh.premises_count,
  poh.description, poh.id_fund_type, 1
FROM _premises_of_hostels4 poh;

-- Объединяем все комнаты в единую таблицу
-- Но прежде корректируем площадь для импорта в поле DOUBLE
UPDATE _rooms_of_hostels3
set total_area = 0
WHERE total_area IS NULL;

INSERT INTO _rooms_total(
  id_premises, sub_premises_number, total_area, description, id_fund_type
  )
SELECT roh.id_premises, 
  roh.sub_premises_number, CAST(REPLACE(roh.total_area,',','.') AS decimal(10,3)), 
  roh.description, roh.id_fund_type
FROM _rooms_of_hostels3 roh
UNION ALL
SELECT id_premises, sub_premises_number,
  CAST(REPLACE(total_area,',','.') AS decimal(10,3)), description, id_fund_type
FROM _not_privatiz_rooms2;

-- Нормализуем коммерческий фонд единственного частного дома:
INSERT INTO _fund_buildings
  (
    id_fund_type,
    id_building,
    include_restriction_number,
    include_restriction_date,
    include_restriction_description,
    exclude_restriction_number,
    exclude_restriction_date,
    exclude_restriction_description,
    description
  )
SELECT 2, id_building, a.include_number, a.include_date, a.include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description, 
  CONCAT(' || ',c.Наниматель) AS description
FROM _commercial_fund3 c LEFT JOIN (
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post))  AS `include_description`,
TRIM(SUBSTRING(draft_num_date,1, LOCATE('от', draft_num_date) - 2)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date, LOCATE('от', draft_num_date) + 2)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('№', include_restriction) - 1) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('№', include_restriction) + 1)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) NOT REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') a
UNION ALL
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post)) AS `description`,
TRIM(SUBSTRING(draft_num_date, LOCATE('№', draft_num_date) + 1)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date,1, LOCATE('№', draft_num_date) - 1)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('от', include_restriction) - 2) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('от', include_restriction) + 2)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') b) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE premise_number IS NULL;

-- Нормализуем коммерческий фонд квартир
-- Переводим буквы в номерах квартир в нижний регистр
UPDATE _commercial_fund3
set premise_number = LOWER(premise_number);

-- Импортируем проблемные квартиры
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.normalize_commercial_premises()
BEGIN
  -- Объявляем колонки таблицы
Declare id_building_ INT;
Declare include_number_ VARCHAR(255);
Declare include_date_ date;
DECLARE include_description_ varchar(255);
Declare exclude_number_ VARCHAR(255);
Declare exclude_date_ date;
DECLARE exclude_description_ varchar(255);
DECLARE description_ varchar(255);
DECLARE premises_number_ varchar(255);
DECLARE premises_number_list varchar(255);
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT id_building, a.include_number, a.include_date, a.include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description, 
  c.premise_number, 
  CONCAT(c.description, IF(c.exclude_from_commercial IS NULL,'',
    CONCAT(' || ', c.exclude_from_commercial))) AS description
FROM _commercial_fund3 c LEFT JOIN (
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post))  AS `include_description`,
TRIM(SUBSTRING(draft_num_date,1, LOCATE('от', draft_num_date) - 2)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date, LOCATE('от', draft_num_date) + 2)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('№', include_restriction) - 1) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('№', include_restriction) + 1)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) NOT REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') a
UNION ALL
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post)) AS `description`,
TRIM(SUBSTRING(draft_num_date, LOCATE('№', draft_num_date) + 1)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date,1, LOCATE('№', draft_num_date) - 1)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('от', include_restriction) - 2) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('от', include_restriction) + 2)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') b) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE premise_number IS NOT NULL AND
  premise_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$'
  AND premise_number NOT REGEXP '^[0-9]+[аб]*$'
  AND c.premise_number REGEXP '^[0-9]+[аб]*([,][0-9])+'
  AND a.id_record NOT IN (297, 382, 477, 701);
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_building_,
    include_number_,
    include_date_,
    include_description_,
    exclude_number_, 
    exclude_date_,
    exclude_description_,
    premises_number_list,
    description_;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE premises_number_list <> '' DO
    set premises_number_ = 
      SUBSTRING(premises_number_list, 1, LOCATE(',',premises_number_list) - 1);
    IF (premises_number_ = '') THEN
      set premises_number_ = premises_number_list;
      set premises_number_list = '';
    ELSE 
      set premises_number_list = 
        SUBSTRING(premises_number_list, LOCATE(',',premises_number_list) + 1);
    END IF;
    INSERT INTO _fund_premises(id_fund_type, 
      id_building, id_premises, premises_number, include_restriction_number,
      include_restriction_date,include_restriction_description,
      exclude_restriction_number,exclude_restriction_date,exclude_restriction_description,
      description)
    VALUES (2, id_building_, NULL, premises_number_, include_number_, include_date_,
      include_description_,exclude_number_, exclude_date_, exclude_description_,
      description_);
  END WHILE;
END WHILE;
CLOSE cursor1;
COMMIT;
END

--Импортируем непроблемные квартиры
INSERT INTO 
_fund_premises
  (
    id_fund_type,
    id_building,
    id_premises,
    premises_number,
    include_restriction_number,
    include_restriction_date,
    include_restriction_description,
    exclude_restriction_number,
    exclude_restriction_date,
    exclude_restriction_description,
    description
  )
SELECT 2, id_building, NULL, c.premise_number, a.include_number, a.include_date, a.include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description, 
  c.description
FROM _commercial_fund3 c LEFT JOIN (
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post))  AS `include_description`,
TRIM(SUBSTRING(draft_num_date,1, LOCATE('от', draft_num_date) - 2)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date, LOCATE('от', draft_num_date) + 2)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('№', include_restriction) - 1) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('№', include_restriction) + 1)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) NOT REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') a
UNION ALL
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post)) AS `description`,
TRIM(SUBSTRING(draft_num_date, LOCATE('№', draft_num_date) + 1)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date,1, LOCATE('№', draft_num_date) - 1)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('от', include_restriction) - 2) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('от', include_restriction) + 2)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') b) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE premise_number IS NOT NULL AND
  premise_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$'
  AND (premise_number REGEXP '^[0-9]+[аб]*$' OR
  a.id_record IN (297, 382, 477, 701));
  
-- Нормализуем привязки коммерческого фонда к комнатам
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.normalize_commercial_sub_premises()
BEGIN
  -- Объявляем колонки таблицы
Declare id_building_ INT;
Declare include_number_ VARCHAR(255);
Declare include_date_ date;
DECLARE include_description_ varchar(255);
Declare exclude_number_ VARCHAR(255);
Declare exclude_date_ date;
DECLARE exclude_description_ varchar(255);
DECLARE description_ varchar(255);
DECLARE premises_number_ varchar(255);
DECLARE sub_premises_number_ varchar(255);
DECLARE sub_premises_number_list varchar(255);
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT id_building,
  SUBSTRING(c.premise_number,1,LOCATE('(',c.premise_number)-1) AS premise_number, 
  SUBSTRING(c.premise_number,LOCATE('(',c.premise_number)+1,LOCATE(')',c.premise_number)-LOCATE('(',c.premise_number)-1) AS rooms,
  a.include_number, a.include_date, a.include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description, 
  c.description
FROM _commercial_fund3 c LEFT JOIN (
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post))  AS `include_description`,
TRIM(SUBSTRING(draft_num_date,1, LOCATE('от', draft_num_date) - 2)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date, LOCATE('от', draft_num_date) + 2)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('№', include_restriction) - 1) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('№', include_restriction) + 1)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) NOT REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') a
UNION ALL
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post)) AS `description`,
TRIM(SUBSTRING(draft_num_date, LOCATE('№', draft_num_date) + 1)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date,1, LOCATE('№', draft_num_date) - 1)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('от', include_restriction) - 2) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('от', include_restriction) + 2)) AS draft_num_date
FROM _commercial_fund3
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') b) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _commercial_fund3
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE premise_number IS NOT NULL AND
  premise_number REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$';
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_building_,
    premises_number_,
    sub_premises_number_list,
    include_number_,
    include_date_,
    include_description_,
    exclude_number_, 
    exclude_date_,
    exclude_description_,
    description_;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE sub_premises_number_list <> '' DO
    set sub_premises_number_ = 
      SUBSTRING(sub_premises_number_list, 1, LOCATE(',',sub_premises_number_list) - 1);
    IF (sub_premises_number_ = '') THEN
      set sub_premises_number_ = sub_premises_number_list;
      set sub_premises_number_list = '';
    ELSE 
      set sub_premises_number_list = 
        SUBSTRING(sub_premises_number_list, LOCATE(',',sub_premises_number_list) + 1);
    END IF;
    INSERT INTO _fund_sub_premises(id_fund_type, 
      id_building, premise_number, id_sub_premises, sub_premise_number, include_restriction_number,
      include_restriction_date,include_restriction_description,
      exclude_restriction_number,exclude_restriction_date,exclude_restriction_description,
      description)
    VALUES (2, id_building_, premises_number_, NULL, sub_premises_number_, include_number_, 
      include_date_, include_description_,exclude_number_, exclude_date_, exclude_description_,
      description_);
  END WHILE;
END WHILE;
CLOSE cursor1;
COMMIT;
END

-- Запрос поиска дубликатов квартир
SELECT *
FROM _premises_total p1
WHERE (SELECT COUNT(*) FROM _premises_total p2 
        WHERE p2.id_building = p1.id_building AND p2.flat_number = p1.flat_number AND flat_number <> 0
        GROUP BY id_building, flat_number) > 1;

-- Делаем привязки квартир по коммерческому фонду
-- Привязалось 687 объектов из 830
-- Перечень незавязанных помещений придется разрулить вручную
UPDATE _fund_premises
SET id_premises =
(SELECT id_record
   FROM _premises_total pt
   WHERE pt.flat_number <> '0' AND
    pt.id_record NOT IN (14001, 14822, 14520, 14575, 14719, 14816, 9773, 9765, 14203, 4716, 15029, 9768, 14439) AND
    pt.id_building = _fund_premises.id_building AND
    pt.flat_number = _fund_premises.premises_number);

-- Делаем привязки комнат по коммерческому фонду
-- Привязалось 35 строк по идентификатору квартиры из 38
UPDATE _fund_sub_premises
SET id_premises =
(SELECT id_record
   FROM _premises_total pt
   WHERE pt.flat_number <> '0' AND
    pt.id_record NOT IN (14001, 14822, 14520, 14575, 14719, 14816, 9773, 9765, 14203, 4716, 15029, 9768, 14439) AND
    pt.id_building = _fund_sub_premises.id_building AND
    pt.flat_number = _fund_sub_premises.premise_number);
-- Привязалось 27 строк по идентификатору квартиры и комнаты из 38
-- Перечень незавязанных комнат придется разрулить вручную
UPDATE _fund_sub_premises
SET id_sub_premises =
(SELECT id_room
   FROM _rooms_total pt
   WHERE 
    pt.id_premises = _fund_sub_premises.id_premises AND
    pt.sub_premises_number = _fund_sub_premises.sub_premise_number AND
    pt.id_room NOT IN (298,377, 104, 378, 105, 379, 380));

-------------------------------------------
-- Обрабатываем записи вторичного включения
-- Объединяем записи обоих постановлений в одну таблицу
INSERT INTO
  _commercial_fund_second_include
SELECT _commercial_fund_second_include_3409.*, '3409'
FROM _commercial_fund_second_include_3409
UNION ALL
SELECT _commercial_fund_second_include_3550.*, '3550'
FROM _commercial_fund_second_include_3550;

-- Делаем привязку улиц
UPDATE _commercial_fund_second_include
set id_street_prefix = 'ул.';

UPDATE _commercial_fund_second_include
set id_street_prefix = 'пер.',
  `Адрес, г. Братск, ул. (б, пер)` = 'Лазурный'
WHERE `Адрес, г. Братск, ул. (б, пер)` = 'Лазурный, пер.' AND `ж.р.` = 'пад';

UPDATE registry._commercial_fund_second_include
set id_street = 
(SELECT DISTINCT id_street
FROM (
  SELECT `Адрес, г. Братск, ул. (б, пер)` AS address, id_street_prefix, `№ дома`,
    CASE `ж.р.` 
    WHEN 'ч' THEN 'жилрайон. Чекановский'
    WHEN 'ц' THEN 'жилрайон. Центральный'
    WHEN 'о' THEN 'жилрайон. Осиновка'
    WHEN 'пор' THEN 'жилрайон. Порожский'
    WHEN 'пад' THEN 'жилрайон. Падун'
    WHEN 'э' THEN 'жилрайон. Энергетик'
    WHEN 'г' THEN 'жилрайон. Гидростроитель'
    WHEN 'с' THEN 'жилрайон. Сухой'
    WHEN 'б' THEN 'жилрайон. Бикей'
    END AS region, `ж.р.`
FROM _commercial_fund_second_include) a INNER JOIN v_kladr_streets ON
CONCAT(region,', ',CONCAT(id_street_prefix,' ',address)) = v_kladr_streets.street_name
WHERE a.address = _commercial_fund_second_include.`Адрес, г. Братск, ул. (б, пер)` AND
  a.`№ дома` = _commercial_fund_second_include.`№ дома` AND a.`ж.р.` = _commercial_fund_second_include.`ж.р.`);
  
-- Делаем привязку к зданию и перенос на следующий уровень обработки
UPDATE _commercial_fund_second_include
set `№ дома` = LOWER(`№ дома`);

UPDATE _commercial_fund_second_include 
set id_building = 
  (SELECT id_record
  FROM _not_privatiz_houses2
  WHERE _not_privatiz_houses2.id_street = _commercial_fund_second_include.id_street
  AND _not_privatiz_houses2.house = _commercial_fund_second_include.`№ дома`)

-- Переводим вторичные включения в коммерческий фонд на 2 шаг
INSERT INTO _commercial_fund_second_include2
  (
  n_post,
  id_building,
  premise_number,
  description,
  include_restriction,
  exclude_restriction,
  post_number
  )
SELECT `№ п/п в постановлении`, id_building, `№ кв.`, Примечание,
`Вкл вторично`, `Искл из коммерческого фонда ( постановление от- №) и др. изм`,
`Номер постановления`
FROM _commercial_fund_second_include1;

-- Импортируем вторичные включения
UPDATE _commercial_fund_second_include2
set premise_number = LOWER(premise_number);

-- Тут я забыл скопировать импорт единственного дома

-- Импорт вторичных включений квартир коммерческого фонда
INSERT INTO 
_fund_premises
  (
    id_fund_type,
    id_building,
    id_premises,
    premises_number,
    include_restriction_number,
    include_restriction_date,
    include_restriction_description,
    exclude_restriction_number,
    exclude_restriction_date,
    exclude_restriction_description,
    description
  )
SELECT 2, id_building, NULL, c.premise_number, a.include_number, a.include_date, a.include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description,
  CONCAT(IFNULL(c.description,''),' || № постановления ',IFNULL(c.post_number,'')) AS description
FROM _commercial_fund_second_include2 c LEFT JOIN (
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post))  AS `include_description`,
TRIM(SUBSTRING(draft_num_date,1, LOCATE('от', draft_num_date) - 2)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date, LOCATE('от', draft_num_date) + 2)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('№', include_restriction) - 1) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('№', include_restriction) + 1)) AS draft_num_date
FROM _commercial_fund_second_include2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) NOT REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') a
UNION ALL
SELECT id_record, include_restriction, 
  CONCAT(`description`,' || № по постановлению: ',TRIM('.' FROM n_post)) AS `description`,
TRIM(SUBSTRING(draft_num_date, LOCATE('№', draft_num_date) + 1)) AS include_number,
STR_TO_DATE(TRIM(SUBSTRING(draft_num_date,1, LOCATE('№', draft_num_date) - 1)),'%d.%m.%Y') AS include_date
FROM (
SELECT id_record, n_post, include_restriction,
  SUBSTRING(include_restriction, 1, LOCATE('от', include_restriction) - 2) AS `description`,
  TRIM(SUBSTRING(include_restriction, LOCATE('от', include_restriction) + 2)) AS draft_num_date
FROM _commercial_fund_second_include2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9][0-9].[0-9][0-9].[0-9][0-9][0-9][0-9][ ]*№[ ]*[0-9]*$') b) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _commercial_fund_second_include2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _commercial_fund_second_include2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE premise_number IS NOT NULL AND
  premise_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$'
  AND premise_number REGEXP '^[0-9]+[аб]*$';
  
-- Заносим вручную пару записей (сдвоиная запись по квартире и запись по комнате)
-- Не стал писать скрипт, потому что всего 2 записи невалидных
-- Комнатe из вторичной привязки не удалось связать с существующей
-- комнатой в базе. Номер добавлен в файл неизвестных привязок коммерческого фонда

-- Привязываем добавленные данные в таблицу _fund_premises
-- к идентификаторам комнат
-- Привязано 66 записей из 91
-- Не привязано 25 записей
UPDATE _fund_premises
SET id_premises =
(SELECT id_record
   FROM _premises_total pt
   WHERE pt.flat_number <> '0' AND
    pt.id_record NOT IN (14001, 14822, 14520, 14575, 14719, 14816, 9773, 9765, 14203, 4716, 15029, 9768, 14439) AND
    pt.id_building = _fund_premises.id_building AND
    pt.flat_number = _fund_premises.premises_number)
WHERE id_fund >= 1060

-- Объединяем таблицы "служебных помещений" в одну
INSERT INTO _is_official_premises
SELECT *, 'central'
FROM _is_official_premises_central
UNION ALL
SELECT *, 'other'
FROM _is_offecial_premises_other;

-- Валидируем адреса
UPDATE _is_official_premises
  set house = TRIM(house),
  street = TRIM(street),
  premises = TRIM(premises);
  
UPDATE _is_official_premises
set street_prefix = 'ул.';

UPDATE _is_official_premises
set street = 'К.Маркса'
WHERE street = 'Карла Маркса';

UPDATE _is_official_premises
set street = 'Победы',
  street_prefix = 'б-р.'
WHERE street = '30 лет Победы';

UPDATE _is_official_premises
set street = 'Ленина',
  street_prefix = 'пр-кт.'
WHERE street = 'Ленина';

UPDATE _is_official_premises
set street_prefix = 'б-р.'
WHERE street = 'Космонавтов';

UPDATE _is_official_premises
set street_prefix = 'пер.',
  street = 'Звездный 2-й'
WHERE street = 'пер.2-й Звездный';

UPDATE _is_official_premises
set 
  street = '25-летия Братскгэсстроя'
WHERE street = '25-лет БГС';

UPDATE _is_official_premises
set 
  street = '25 Партсъезда'
WHERE street = '25Партсъезда';

UPDATE _is_official_premises
set 
  street = 'Новый',
  street_prefix = 'пер.'
WHERE street = 'пер.Новый';

UPDATE _is_official_premises
set 
  street = 'Воинов-интернационалистов'
WHERE street = 'В-Интернационалистов';

UPDATE _is_official_premises
set 
  street_prefix = 'пер.',
  street = 'Лазурный'
WHERE street = 'пер.Лазурный';

UPDATE _is_official_premises
set 
  street = '40 лет Победы'
WHERE street = 'б.40-лет Победы' OR street = '40-лет Победы';

UPDATE _is_official_premises
set 
  street_prefix='б-р.',
  street = 'Победы'
WHERE street = '30-лет Победы';

UPDATE _is_official_premises
set 
  street_prefix='б-р.',
  street = 'Победы'
WHERE street = '30 лет Победы';

UPDATE _is_official_premises
set 
  street_prefix = 'пер.',
  street = 'Дубынинский'
WHERE street = 'пер.Дубынинский';

UPDATE _is_official_premises
set 
  street = 'Парковая 1-я'
WHERE street = '1-я Парковая';

UPDATE _is_official_premises
set 
  street_prefix='пер.',
  street = 'Парковый 1-я'
WHERE street = '1-й Парковый';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = '25-летия Братскгэсстроя';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Приморская';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Юбилейная';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Клубная';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Зверева';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Гидростроителей';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Погодаева';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Набережная';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Хабарова';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Наймушина';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Юго-западная';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Гиндина';

UPDATE _is_official_premises
set 
  region_marker = 'bikey'
WHERE street = 'Фестивальная';

UPDATE _is_official_premises
set 
  region_marker = 'check'
WHERE street = 'Прибрежная';

UPDATE _is_official_premises
set 
  region_marker = 'bikey'
WHERE street = 'Профсоюзная';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Воинов-интернационалистов';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Макаренко';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Пирогова';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Студенческая';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Мечтателей';

UPDATE _is_official_premises
set 
  region_marker = 'energetic',
  street = 'Холоднова'
WHERE street = 'Зеленая';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Иванова';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Горького';

UPDATE _is_official_premises
set 
  region_marker = 'osin'
WHERE street = 'Спортивная';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Гайнулина';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Островского';

UPDATE _is_official_premises
set 
  region_marker = 'energetic'
WHERE street = 'Олимпийская';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Вокзальная';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Енисейская';

UPDATE _is_official_premises
set 
  region_marker = 'osin'
WHERE street = 'Зябская';

UPDATE _is_official_premises
set 
  region_marker = 'bikey'
WHERE street = 'Лесорубов';

UPDATE _is_official_premises
set 
  region_marker = 'bikey',
  street = 'Орджоникидзе'
WHERE street = 'Орджиникидзе';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Заводская';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Лермонтова';

UPDATE _is_official_premises
set 
  region_marker = 'osin'
WHERE street = 'Заярская';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Полевая';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Байкальская';

UPDATE _is_official_premises
set 
  region_marker = 'osin'
WHERE street = 'Центральная';

UPDATE _is_official_premises
set 
  region_marker = 'osin'
WHERE street = 'Калужская';

UPDATE _is_official_premises
set 
  street = 'Крупской'
WHERE street = 'Крупская';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Лазурный';

UPDATE _is_official_premises
set 
  region_marker = 'padun'
WHERE street = 'Коньшакова';

UPDATE _is_official_premises
set 
  region_marker = 'check'
WHERE street = 'Почтовая';

UPDATE _is_official_premises
set 
  region_marker = 'check'
WHERE street = 'Школьная';

UPDATE _is_official_premises
set 
  region_marker = 'check'
WHERE street = 'Грибная';

UPDATE _is_official_premises
set 
  region_marker = 'check'
WHERE street = 'Первопроходцев';

UPDATE _is_official_premises
set 
  region_marker = 'por'
WHERE street = 'Лесная';

UPDATE _is_official_premises
set 
  region_marker = 'osin'
WHERE street = 'Железнодорожная';

UPDATE _is_official_premises
set 
  region_marker = 'central',
  street = 'Парковая',
  street_prefix = 'ул.'
WHERE street = 'Парковая 1-я' OR street = 'Парковый 1-я';

UPDATE _is_official_premises
set 
  region_marker = 'gidr',
  street = 'Осиновский',
  street_prefix = 'пер.'
WHERE street = 'Осиновской';

UPDATE _is_official_premises
set 
  region_marker = 'padun',
  street_prefix = 'пер.'
WHERE street = 'Пурсей';

UPDATE _is_official_premises
set 
  region_marker = 'central',
  street_prefix = 'туп.',
  street = 'Крылатый'
WHERE street = 'Оздоров.компл.';

UPDATE _is_official_premises
set 
  region_marker = 'osin',
  street = 'Мехколонна 163'
WHERE street = 'МК-163';

UPDATE _is_official_premises
set 
  region_marker = 'gidr',
  street = 'Братское Взморье'
WHERE street = 'сан.Бр.взморье';

UPDATE _is_official_premises
set 
  region_marker = 'gidr',
  street = 'Осиновский 1-й'
WHERE street = 'Осиновский';

UPDATE _is_official_premises
set 
  region_marker = 'gidr'
WHERE street = 'Тургенева';

-- Привязываем идентификаторы улиц
UPDATE _is_official_premises
set id_street =
  (
      SELECT id_street 
      FROM v_kladr_streets vks 
       WHERE vks.street_name LIKE CONCAT('%',
        IF(_is_official_premises.region_marker = 'central','жилрайон. Центральный, ',
        IF(_is_official_premises.region_marker = 'padun','жилрайон. Падун, ',
        IF(_is_official_premises.region_marker = 'energetic','жилрайон. Энергетик, ',
        IF(_is_official_premises.region_marker = 'bikey','жилрайон. Бикей, ',
        IF(_is_official_premises.region_marker = 'check','жилрайон. Чекановский, ',
        IF(_is_official_premises.region_marker = 'gidr','жилрайон. Гидростроитель, ',
        IF(_is_official_premises.region_marker = 'osin','жилрайон. Осиновка, ',
        IF(_is_official_premises.region_marker = 'por','жилрайон. Порожский, ','')))))))), 
        CONCAT(_is_official_premises.street_prefix,' ',_is_official_premises.street))
  )
  
  
-- Привязываем id домов (привязано 1199 записей
-- 149 не привязано)
UPDATE _is_official_premises
set id_building = 
(SELECT id_record
FROM _not_privatiz_houses2 
  WHERE _not_privatiz_houses2.id_street = _is_official_premises.id_street AND 
  _not_privatiz_houses2.house = _is_official_premises.house AND
  _not_privatiz_houses2.id_record NOT IN (2099,3008,2164,2179,2183,357,7,566,909,568));
  
-- Маркируем квартиры, как служебные в соответсвии с информацией из данной таблицы
-- Все квартиры следующих домов (28 квартир помечено):
-- +6 квартир на Весенней 20, !!неоднозначность
UPDATE _premises_total
set is_official_premise = TRUE
WHERE id_building IN (
SELECT id_building
FROM registry._is_official_premises
WHERE id_building IS NOT NULL
  AND premises IS NULL);
  
-- 769 помещений, упомянутых в таблице служебных, нет в базе помещений
SELECT *
FROM registry._is_official_premises
WHERE id_building IS NOT NULL
  AND premises REGEXP '^[0-9]+[абвг]*$'
  AND premises NOT IN (SELECT _premises_total.flat_number 
                        FROM _premises_total 
                        WHERE _premises_total.id_building = _is_official_premises.id_building)

-- Привязываем оставшиеся 316
-- В действительности обновилось 299, 
-- т.к. часть записей уже была обновлена
UPDATE _premises_total
set is_official_premise = TRUE
WHERE (SELECT COUNT(*)
FROM _is_official_premises
WHERE _is_official_premises.id_building IS NOT NULL
  AND _is_official_premises.premises REGEXP '^[0-9]+[абвг]*$'
  AND _is_official_premises.id_building = _premises_total.id_building
  AND _is_official_premises.premises = _premises_total.flat_number) > 0;
  
-- Геморные помещения
-- Совпадение найдено всего по одному помещению...
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.is_official_premises_spliter_updater()
BEGIN
  -- Объявляем колонки таблицы
Declare id_building_ INT;
Declare premises_list VARCHAR(255);
Declare premises_current VARCHAR(255);
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT id_building, premises
      FROM _is_official_premises
      WHERE _is_official_premises.id_building IS NOT NULL
        AND _is_official_premises.premises NOT REGEXP '^[0-9]+[абвг]*$'
        AND _is_official_premises.premises NOT REGEXP '^[0-9]+[абвг]*[(][0-9]+[абвг]*([,][0-9]+[абвг]*)*[)]$'
        AND _is_official_premises.premises REGEXP '^[0-9]+[абвг]*([,][0-9]+[абвг]*)*$';
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION;
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_building_,
    premises_list;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE premises_list <> '' DO
    set premises_current = 
      SUBSTRING(premises_list, 1, LOCATE(',',premises_list) - 1);
    IF (premises_current = '') THEN
      set premises_current = premises_list;
      set premises_list = '';
    ELSE 
      set premises_list = 
        SUBSTRING(premises_list, LOCATE(',',premises_list) + 1);
    END IF; 
    UPDATE _premises_total
    set is_official_premise = TRUE
    WHERE id_building = id_building_ AND flat_number = premises_current;
  END WHILE;
END WHILE;
CLOSE cursor1;
COMMIT;
END

-- 14 комнат являются служебными
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.is_official_sub_premises_spliter_updater()
BEGIN
  -- Объявляем колонки таблицы
Declare id_premise_ INT;
Declare sub_premises_list VARCHAR(255);
Declare sub_premises_current VARCHAR(255);
Declare incrementor int DEFAULT 0;
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT p.id_record, rooms
      FROM (
      SELECT id_building, SUBSTRING(premises,1,LOCATE('(',premises)-1) AS premises,
      SUBSTRING(premises,LOCATE('(',premises)+1,LOCATE(')',premises) - LOCATE('(',premises)-1) AS rooms
      FROM _is_official_premises
      WHERE _is_official_premises.id_building IS NOT NULL
        AND _is_official_premises.premises NOT REGEXP '^[0-9]+[абвг]*$'
        AND _is_official_premises.premises REGEXP '^[0-9]+[абвг]*[(][0-9]+[абвг]*([,][0-9]+[абвг]*)*[)]$'
        AND _is_official_premises.premises NOT REGEXP '^[0-9]+[абвг]*([,][0-9]+[абвг]*)*$') a INNER JOIN
        _premises_total p ON a.id_building = p.id_building AND
        a.premises = p.flat_number;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION;
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_premise_,
    sub_premises_list;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE sub_premises_list <> '' DO
    set sub_premises_current = 
      SUBSTRING(sub_premises_list, 1, LOCATE(',',sub_premises_list) - 1);
    IF (sub_premises_current = '') THEN
      set sub_premises_current = sub_premises_list;
      set sub_premises_list = '';
    ELSE 
      set sub_premises_list = 
        SUBSTRING(sub_premises_list, LOCATE(',',sub_premises_list) + 1);
    END IF; 
    UPDATE _rooms_total
    set is_official_sub_premise = TRUE
    WHERE id_premises = id_premise_ AND sub_premises_number = sub_premises_current;
  END WHILE;
END WHILE;
CLOSE cursor1;
SELECT incrementor;
COMMIT;
END

-- Ставим маркер общежития
UPDATE _premises_total
set id_hostel_type = TRUE
WHERE id_record > 14000

-- Нормализуем адреса спец. фонда
UPDATE _special_fund1
  set street = TRIM(street);
  
-- Прогоняем старые скрипты коррекции адреса по данной таблице

-- Правим оставшиеся 24 улицы
UPDATE _special_fund1
 set street = 'Победы',
  street_prefix = 'б-р.'
WHERE street = 'Бульвар Победы';

UPDATE _special_fund1
 set street = 'Маршала Жукова'
WHERE street = 'Жукова';

UPDATE _special_fund1
 set street = 'Новый',
  street_prefix = 'пер.'
WHERE street = 'Новый пер.';

UPDATE _special_fund1
 set street = '20 Партсъезда',
  region_marker = 'por'
WHERE street = 'ХХ Партсъезда   (ж.р. Порожский)';

UPDATE _special_fund1
set 
  street_prefix = 'туп.',
  street = 'Крылатый'
WHERE street = 'Оздоровительный комплекс БрАЗа';

UPDATE _special_fund1
set 
  street_prefix = 'б-р.'
WHERE street = 'Победы';

UPDATE _special_fund1
set 
  region_marker = 'energetic'
WHERE street = 'Солнечная';

UPDATE _special_fund1
set 
  region_marker = 'energetic'
WHERE street = 'Холоднова';

UPDATE _special_fund1
set 
  region_marker = 'bikey'
WHERE street = 'Орджоникидзе';

UPDATE _special_fund1
set 
  region_marker = 'padun'
WHERE street = 'Весенняя';

UPDATE _special_fund1
set 
  region_marker = 'padun',
  street_prefix = 'пер.'
WHERE street = 'Лазурный';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Сосновая';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Некрасова';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Чехова';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Маяковского';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Радищева';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Грибоедова';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Мало-Амурская';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Краснодарская';

UPDATE _special_fund1
set 
  region_marker = 'suh'
WHERE street = 'Социалистическая';

UPDATE _special_fund1
set 
  region_marker = 'osin'
WHERE street = 'Куйбышевская';

UPDATE _special_fund1
set 
  region_marker = 'gidr'
WHERE street = 'Пушкина';

UPDATE _special_fund1
set 
  region_marker = 'gidr',
  street = 'Звездный 2-й',
  street_prefix = 'пер.'
WHERE street = '2-й Звездный';

-- Проверяем, чтобы запрос ничего не возвращал
SELECT DISTINCT street, CONCAT(
        IF(sf.region_marker = 'central','жилрайон. Центральный, ',
        IF(sf.region_marker = 'padun','жилрайон. Падун, ',
        IF(sf.region_marker = 'energetic','жилрайон. Энергетик, ',
        IF(sf.region_marker = 'bikey','жилрайон. Бикей, ',
        IF(sf.region_marker = 'check','жилрайон. Чекановский, ',
        IF(sf.region_marker = 'gidr','жилрайон. Гидростроитель, ',
        IF(sf.region_marker = 'osin','жилрайон. Осиновка, ',
        IF(sf.region_marker = 'suh','жилрайон. Сухой, ',
        IF(sf.region_marker = 'por','жилрайон. Порожский, ','жилрайон. Центральный, '))))))))), 
        CONCAT(sf.street_prefix,' ',sf.street))
FROM _special_fund1 sf
WHERE street IS NOT NULL AND (SELECT COUNT(*) FROM v_kladr_streets vks 
        WHERE vks.street_name LIKE CONCAT(
        IF(sf.region_marker = 'central','жилрайон. Центральный, ',
        IF(sf.region_marker = 'padun','жилрайон. Падун, ',
        IF(sf.region_marker = 'energetic','жилрайон. Энергетик, ',
        IF(sf.region_marker = 'bikey','жилрайон. Бикей, ',
        IF(sf.region_marker = 'check','жилрайон. Чекановский, ',
        IF(sf.region_marker = 'gidr','жилрайон. Гидростроитель, ',
        IF(sf.region_marker = 'osin','жилрайон. Осиновка, ',
        IF(sf.region_marker = 'suh','жилрайон. Сухой, ',
        IF(sf.region_marker = 'por','жилрайон. Порожский, ','жилрайон. Центральный, '))))))))), 
        CONCAT(sf.street_prefix,' ',sf.street))) <> 1
		
-- Делаем привязку адресов к кладеру
UPDATE _special_fund1
set id_street =
  (
      SELECT id_street 
      FROM v_kladr_streets vks 
       WHERE vks.street_name LIKE CONCAT(
        IF(_special_fund1.region_marker = 'central','жилрайон. Центральный, ',
        IF(_special_fund1.region_marker = 'padun','жилрайон. Падун, ',
        IF(_special_fund1.region_marker = 'energetic','жилрайон. Энергетик, ',
        IF(_special_fund1.region_marker = 'bikey','жилрайон. Бикей, ',
        IF(_special_fund1.region_marker = 'check','жилрайон. Чекановский, ',
        IF(_special_fund1.region_marker = 'gidr','жилрайон. Гидростроитель, ',
        IF(_special_fund1.region_marker = 'osin','жилрайон. Осиновка, ',
        IF(_special_fund1.region_marker = 'suh','жилрайон. Сухой, ',
        IF(_special_fund1.region_marker = 'por','жилрайон. Порожский, ','жилрайон. Центральный, '))))))))), 
        CONCAT(_special_fund1.street_prefix,' ',_special_fund1.street))
  )
  
  -- Привязываем id домов привязано 1199 записей
  -- 7 не привязалось
UPDATE _special_fund1
set id_building = 
(SELECT id_record
FROM _not_privatiz_houses2 
  WHERE _not_privatiz_houses2.id_street = _special_fund1.id_street AND 
  _not_privatiz_houses2.house = _special_fund1.house AND
  _not_privatiz_houses2.id_record NOT IN (2099,3008,2164,2179,2183,357,7,566,909,568));
 
 -- Нормализуем номера квартир
UPDATE _special_fund1
set flat_number = TRIM(flat_number);

UPDATE _special_fund1
  set flat_number = LOWER(flat_number);
  
-- Куча запросов нормализации номеров квартир ~ 300 строк

-- 182 записи о специализированном жилом фонде не привязались по ID квартиры
SELECT *
FROM _special_fund1 sf LEFT JOIN
  _premises_total pt ON sf.id_building = pt.id_building AND
  sf.flat_number = pt.flat_number AND
  pt.id_record NOT IN (
14001,
14822,
14520,
14575,
14719,
14816,
9773,
9765,
14203,
4716,
15029,
9768,
14439
  )
WHERE sf.flat_number IS NOT NULL AND
  (sf.flat_number REGEXP '^[0-9]+[абвг]*$' OR
  sf.flat_number NOT REGEXP '^[0-9]+[абвг]*[(][0-9]*[абвг]*([,][0-9]*[абвг]*)*[)]$')
  AND pt.id_record IS NULL;
  
-- Приводим в порядок реквизиты
UPDATE _special_fund1
  set include_exclude_restrictions = TRIM(include_exclude_restrictions);
 
-- Переводим спец. фонд на следующий этап
 
-- Продолжаем нормализацию
UPDATE _special_fund2
set include_exclude_restrictions = REPLACE(include_exclude_restrictions,'  ',' ');

-- Расщипляем реквизиты на вкл. и искл

UPDATE _special_fund2
set include_exclude_restrictions = REPLACE(include_exclude_restrictions, 'отн','вкл')
WHERE LOCATE('отн',include_exclude_restrictions) <> 0;

UPDATE _special_fund2
set include_exclude_restrictions = REPLACE(include_exclude_restrictions, 'вкл','')
WHERE LOCATE('вкл',include_exclude_restrictions) <> 0;

UPDATE _special_fund2
set include_exclude_restrictions = TRIM(',' FROM include_exclude_restrictions);

UPDATE _special_fund2
set include_exclude_restrictions = TRIM(include_exclude_restrictions);

UPDATE _special_fund2
set exclude_restriction = include_exclude_restrictions,
  include_exclude_restrictions = ''
WHERE include_exclude_restrictions LIKE '%искл%'
  AND LOCATE('искл',include_exclude_restrictions) = 1;
  
UPDATE _special_fund2
set exclude_restriction = SUBSTRING(include_exclude_restrictions, 
  LOCATE('искл',include_exclude_restrictions)),
  include_exclude_restrictions = REPLACE(include_exclude_restrictions, SUBSTRING(include_exclude_restrictions, 
  LOCATE('искл',include_exclude_restrictions)),'')
WHERE include_exclude_restrictions LIKE '%искл%'
  AND LOCATE('искл',include_exclude_restrictions) > 1;
  
UPDATE _special_fund2
set include_exclude_restrictions = TRIM(TRIM(',' FROM TRIM(include_exclude_restrictions)));

UPDATE _special_fund2
set include_exclude_restrictions = REPLACE(include_exclude_restrictions,'от ','');

UPDATE _special_fund2
set include_restriction = include_exclude_restrictions
WHERE include_exclude_restrictions IS NOT NULL AND 
  include_exclude_restrictions <> '';
  
UPDATE _special_fund2
set exclude_restriction = REPLACE(exclude_restriction,'искл.','')
WHERE exclude_restriction IS NOT NULL;

UPDATE _special_fund2
set exclude_restriction = REPLACE(exclude_restriction,'искл','')
WHERE exclude_restriction IS NOT NULL;

UPDATE _special_fund2
set exclude_restriction = TRIM(TRIM(',' FROM TRIM(exclude_restriction)))
WHERE exclude_restriction IS NOT NULL;

UPDATE _special_fund2
set include_restriction=REPLACE(include_restriction,'.',''),
  exclude_restriction=REPLACE(exclude_restriction,'.','');
  
UPDATE _special_fund2
set include_restriction=REPLACE(include_restriction,' № ','-'),
  exclude_restriction=REPLACE(exclude_restriction,' № ','-');
  
-- Перечень реквизитов, которые не удалось распарсить (их всего 3):
-- Дома-квартиры: 738-108, 406-25, 392-106
SELECT 3, id_building, NULL, c.flat_number, a.include_number, a.include_date, NULL AS include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description,
  c.description, c.include_restriction, c.exclude_restriction
FROM _special_fund2 c LEFT JOIN (
  SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 10) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), LOCATE('-', TRIM(include_restriction)) + 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 1, LOCATE('-', TRIM(include_restriction)) - 1) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9]{8}$'
) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE flat_number IS NOT NULL AND
  flat_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$'
  AND flat_number REGEXP '^[0-9]+[абвг]*$'
  AND ((c.include_restriction IS NOT NULL AND a.include_date IS NULL) OR 
        (c.exclude_restriction IS NOT NULL AND b.exclude_date IS NULL))

-- Вставляем спец. фонд квартир в таблицу фондов квартир
INSERT INTO _fund_premises (
id_fund_type,
id_building,
id_premises,
premises_number,
include_restriction_number,
include_restriction_date,
include_restriction_description,
exclude_restriction_number,
exclude_restriction_date,
exclude_restriction_description,
description
)
SELECT 3, id_building, NULL, c.flat_number, a.include_number, a.include_date, NULL AS include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description,
  c.description
FROM _special_fund2 c LEFT JOIN (
  SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 10) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), LOCATE('-', TRIM(include_restriction)) + 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 1, LOCATE('-', TRIM(include_restriction)) - 1) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9]{8}$'
) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE flat_number IS NOT NULL AND
  flat_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$'
  AND flat_number REGEXP '^[0-9]+[абвг]*$'
  AND ((c.include_restriction IS NOT NULL AND a.include_date IS NOT NULL) OR 
        (c.exclude_restriction IS NOT NULL AND b.exclude_date IS NOT NULL))
		
-- Вставляем 3 нераспарсенных строки
INSERT INTO _fund_premises (
id_fund_type,
id_building,
id_premises,
premises_number,
include_restriction_number,
include_restriction_date,
include_restriction_description,
exclude_restriction_number,
exclude_restriction_date,
exclude_restriction_description,
description
)
SELECT 3, id_building, NULL, c.flat_number, a.include_number, a.include_date, c.include_restriction AS include_description,
  b.exclude_number, b.exclude_date, c.exclude_restriction AS exclude_description,
  c.description
FROM _special_fund2 c LEFT JOIN (
  SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 10) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), LOCATE('-', TRIM(include_restriction)) + 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 1, LOCATE('-', TRIM(include_restriction)) - 1) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9]{8}$'
) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE flat_number IS NOT NULL AND
  flat_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$'
  AND flat_number REGEXP '^[0-9]+[абвг]*$'
  AND ((c.include_restriction IS NOT NULL AND a.include_date IS NULL) OR 
        (c.exclude_restriction IS NOT NULL AND b.exclude_date IS NULL))
		
-- Импортируем привязки спец. фонда к комнатам
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.normalize_special_sub_premises()
BEGIN
  -- Объявляем колонки таблицы
Declare id_building_ INT;
Declare include_number_ VARCHAR(255);
Declare include_date_ date;
DECLARE include_description_ varchar(255);
Declare exclude_number_ VARCHAR(255);
Declare exclude_date_ date;
DECLARE exclude_description_ varchar(255);
DECLARE description_ varchar(255);
DECLARE premises_number_ varchar(255);
DECLARE sub_premises_number_ varchar(255);
DECLARE sub_premises_number_list varchar(255);
  -- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT id_building, 
  SUBSTRING(c.flat_number,1,LOCATE('(',c.flat_number)-1) AS flat_number, 
  SUBSTRING(c.flat_number,LOCATE('(',c.flat_number)+1,LOCATE(')',c.flat_number)-LOCATE('(',c.flat_number)-1) AS rooms,
  a.include_number, a.include_date, NULL AS include_description,
  b.exclude_number, b.exclude_date, NULL AS exclude_description,
  c.description
FROM _special_fund2 c LEFT JOIN (
  SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 10) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), LOCATE('-', TRIM(include_restriction)) + 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 1, LOCATE('-', TRIM(include_restriction)) - 1) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9]{8}$'
) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE flat_number IS NOT NULL AND
  flat_number REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$';
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_building_,
    premises_number_,
    sub_premises_number_list,
    include_number_,
    include_date_,
    include_description_,
    exclude_number_, 
    exclude_date_,
    exclude_description_,
    description_;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  WHILE sub_premises_number_list <> '' DO
    set sub_premises_number_ = 
      SUBSTRING(sub_premises_number_list, 1, LOCATE(',',sub_premises_number_list) - 1);
    IF (sub_premises_number_ = '') THEN
      set sub_premises_number_ = sub_premises_number_list;
      set sub_premises_number_list = '';
    ELSE 
      set sub_premises_number_list = 
        SUBSTRING(sub_premises_number_list, LOCATE(',',sub_premises_number_list) + 1);
    END IF;
    INSERT INTO _fund_sub_premises(id_fund_type, 
      id_building, premise_number, id_sub_premises, sub_premise_number, include_restriction_number,
      include_restriction_date,include_restriction_description,
      exclude_restriction_number,exclude_restriction_date,exclude_restriction_description,
      description)
    VALUES (3, id_building_, premises_number_, NULL, sub_premises_number_, include_number_, 
      include_date_, include_description_,exclude_number_, exclude_date_, exclude_description_,
      description_);
  END WHILE;
END WHILE;
CLOSE cursor1;
COMMIT;
END

-- Вставляем записи принадлежности к соц фонду по группам квартир
INSERT INTO _fund_premises (
id_fund_type,
id_building,
id_premises,
premises_number,
include_restriction_number,
include_restriction_date,
include_restriction_description,
exclude_restriction_number,
exclude_restriction_date,
exclude_restriction_description,
description
)
SELECT 3, id_building, NULL, c.flat_number, a.include_number, a.include_date, c.include_restriction AS include_description,
  b.exclude_number, b.exclude_date, c.exclude_restriction AS exclude_description,
  c.description
FROM _special_fund2 c LEFT JOIN (
  SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 10) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(include_restriction), LOCATE('-', TRIM(include_restriction)) + 1, 8),'%d%m%Y') AS include_date,
  SUBSTRING(TRIM(include_restriction), 1, LOCATE('-', TRIM(include_restriction)) - 1) AS include_number
FROM _special_fund2
WHERE include_restriction IS NOT NULL AND 
  include_restriction <> ''
AND TRIM(include_restriction) REGEXP '[0-9]{8}$'
) a
  ON c.id_record = a.id_record
LEFT JOIN
(SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 10) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '^[0-9]{8}'
UNION ALL
SELECT id_record,
  STR_TO_DATE(SUBSTRING(TRIM(exclude_restriction), LOCATE('-', TRIM(exclude_restriction)) + 1, 8),'%d%m%Y') AS exclude_date,
  SUBSTRING(TRIM(exclude_restriction), 1, LOCATE('-', TRIM(exclude_restriction)) - 1) AS exclude_number
FROM _special_fund2
WHERE exclude_restriction IS NOT NULL AND 
  exclude_restriction <> ''
AND TRIM(exclude_restriction) REGEXP '[0-9]{8}$') b
ON c.id_record = b.id_record
WHERE flat_number IS NOT NULL AND
  flat_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]+[аб]*[,]*)+[)]$'
  AND flat_number NOT REGEXP '^[0-9]+[абвг]*$'
  AND flat_number NOT REGEXP '^[0-9]+[аб]*[(]([0-9]*[аб]*[,]*)+[)]$'

-- Привязываем добавленные данные в таблицу _fund_premises
-- к идентификаторам квартир
-- 769 записей привязалось из 950
UPDATE _fund_premises
SET id_premises =
(SELECT id_record
   FROM _premises_total pt
   WHERE pt.flat_number <> '0' AND
    pt.id_record NOT IN (14001, 14822, 14520, 14575, 14719, 14816, 9773, 9765, 14203, 4716, 15029, 9768, 14439) AND
    pt.id_building = _fund_premises.id_building AND
    pt.flat_number = _fund_premises.premises_number)
WHERE id_fund_type = 3;

-- Привязалось 238 строк по идентификатору квартиры, 26 не привязалось
-- Перечень незавязанных комнат придется разрулить вручную
UPDATE _fund_sub_premises
SET id_sub_premises =
(SELECT id_room
   FROM _rooms_total pt
   WHERE 
    pt.id_premises = _fund_sub_premises.id_premises AND
    pt.sub_premises_number = _fund_sub_premises.sub_premise_number AND
    pt.id_room NOT IN (298,377, 104, 378, 105, 379, 380))
WHERE id_fund_type = 3;

-- Привязалось 175 строк по идентификатору квартиры и комнаты, 89 не привязалось
-- Перечень незавязанных комнат придется разрулить вручную
UPDATE _fund_sub_premises
SET id_sub_premises =
(SELECT id_room
   FROM _rooms_total pt
   WHERE 
    pt.id_premises = _fund_sub_premises.id_premises AND
    pt.sub_premises_number = _fund_sub_premises.sub_premise_number AND
    pt.id_room NOT IN (298,377, 104, 378, 105, 379, 380))
WHERE id_fund_type = 3;

-------------------------------
-- Выполняем конечный импорт --
-------------------------------
INSERT INTO buildings
SELECT id_record AS id_building, IF(is_demolished = '1',2, 1) AS id_state,
  1 AS id_structure_type, id_street, IFNULL(house,'б/н'), 0 AS floors,
  IFNULL(total_flats,0) AS num_premises,
  0 AS rooms, 0 AS num_apartments,
  0 AS num_shared_apartments,
  0 AS living_area,
  '' AS cadastral_num,
  0 AS cadastral_cost,
  0 AS balance_cost,
  description,
  1900 AS startup_year,
  1 AS improvement,
  0 AS elevator,
  0 AS deleted
FROM _not_privatiz_houses2;

INSERT INTO premises
SELECT id_record AS id_premises, id_building,
  IF(id_building IN (
  SELECT nph.id_record 
  FROM _not_privatiz_houses2 nph
  WHERE nph.is_demolished IS NOT NULL), 2, 
  IF(crash_bit = 'с', 2, IF(total_flat_count IS NOT NULL AND total_flat_count != '0',5, 1))) AS id_state,
  flat_number,
  IFNULL(total_area,0),
  IFNULL(living_area,0),
  0 AS num_beds,
  1 AS id_premises_type,
  IF (is_official_premise = 1 AND id_hostel_type = 1, 4, IF(is_official_premise = 1, 2,
  IF(id_hostel_type = 1, 3, 1))) AS id_premises_kind,
  IFNULL(`floor`,0) AS `floor`,
  0 AS for_orphans,
  0 AS accepted_by_exchange,
  0 AS accepted_by_donation,
  0 AS accepted_by_other,
  '' AS cadastral_num,
  0 AS cadastral_cost,
  0 AS balance_cost,
  description,
  0 AS deleted
FROM _premises_total;

INSERT INTO sub_premises
SELECT id_room, id_premises, 1 AS id_state,
  sp.sub_premises_number,
  total_area,
  sp.description,
  0 AS deleted
FROM _rooms_total sp;

-- buildings ownerships
CREATE DEFINER = 'PWR\\IGNVV'@'%'
PROCEDURE registry.insert_buildings_ownerships()
BEGIN
  -- Объявляем колонки таблицы
Declare id_building_ INT;
DECLARE id_ownership_ int;
DECLARE id_ownership_type_ varchar(255);
DECLARE number_ varchar(255);
DECLARE date_ datetime;
DECLARE description_ varchar(255);
-- Объявляем курсок
DECLARE done integer default 0;
DECLARE cursor1 CURSOR FOR
      SELECT id_building, id_ownership_type, NULL AS number, `date`, NULL AS description
FROM _not_privatiz_buildings_ownerships;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done=1;
SET autocommit = 0;
START TRANSACTION; 
OPEN cursor1;
circle1: WHILE done = 0 DO 
  FETCH cursor1 INTO 
    id_building_,
    id_ownership_type_,
    number_,
    date_,
    description_;
  IF (done = 1) THEN
    LEAVE circle1;
  END IF;
  INSERT INTO ownership_rights
    (id_ownership_right_type, number,`date`,description)
  VALUES (id_ownership_type_, number_, date_, description_);
  SET id_ownership_ = LAST_INSERT_ID();
  INSERT INTO ownership_buildings_assoc(id_building, id_ownership_right)
  VALUES(id_building_, id_ownership_);
END WHILE;
CLOSE cursor1;
COMMIT;
END

--premises_ownerships
