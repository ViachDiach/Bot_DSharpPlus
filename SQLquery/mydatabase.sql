
////////////////////_ZAMAT_/////////////////////////////////
CREATE table BotuserData(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    author VARCHAR(30),
    authorid VARCHAR(30),
    procent int,
    curse VARCHAR(30)
    );

INSERT INTO BotuserData (author, authorid, procent, curse) VALUES ('graf_donbasskiy','00000', '14', 'КРУТОЙ ЧЕЛ');

UPDATE BotuserData SET procent = 14 WHERE  author = 'graf_donbasskiy';



SELECT * FROM BotuserData;
SELECT * FROM BotuserData WHERE  author = 'graf_donbasskiy';  


UPDATE BotuserData SET curse = 'не пердун' WHERE  author = 'graf_donbasskiy';
DELETE  FROM BotuserData WHERE author = 'graf_donbasskiy';
DROP TABLE BotuserData;

//////////////////////_MONEY_/////////////////////////////

CREATE table moneyRates(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    currency VARCHAR(5),
    value REAL
    );

INSERT INTO moneyRates (currency, value) VALUES ('kzt', 0.10);
UPDATE moneyRates SET currency = 0,17  WHERE  value = 'kzt';

SELECT * FROM moneyRates;

DROP TABLE moneyRates;

///////////////////////_COFEE_EVENTS////////////////////////////


CREATE table CoffeeMachine(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    goods VARCHAR(30),
    price int,
    url VARCHAR(100)
    );

INSERT INTO CoffeeMachine (goods, price, url) VALUES ('buy_role', 2, 'https://i.gifer.com/origin/09/09d6fe3ed34ada38206b4b890a6e8700_w200.gif');
SELECT * FROM CoffeeMachine WHERE  goods = 'zamat';
SELECT * FROM CoffeeMachine;
UPDATE CoffeeMachine SET price = 3 WHERE goods = 'buy_role';

DROP TABLE CoffeeMachine;




CREATE table Allusers(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    author VARCHAR(30),
    balanse int,
    inventory INTEGER[]
    );

INSERT INTO Allusers (author, balanse) VALUES ('graf_donbasskiy', 13);
SELECT * FROM Allusers;
UPDATE Allusers SET balanse = 114 WHERE  author = 'graf_donbasskiy';
UPDATE Allusers SET balanse = balanse + 0.1 WHERE  author = 'graf_donbasskiy';
UPDATE Allusers SET inventory =  'my test'   WHERE  author = 'graf_donbasskiy';

DELETE FROM Allusers WHERE author = 'graf_donbasskiy';

DROP TABLE Allusers;


 

 ///////////////////////_TEST_////////////////////////////
CREATE TABLE my_table (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
    author VARCHAR(30),
    balanse int,
    inventory INTEGER[]
    );

SELECT * FROM my_table;
INSERT INTO my_table (author, balanse, inventory) VALUES ('test', 2, '{изюм, пианино}');
UPDATE my_table SET inventory = inventory || '{тест}'   WHERE  author = 'test';