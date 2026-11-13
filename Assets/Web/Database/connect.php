<?php
$host = 'mysql-2763091d-plandelman.e.aivencloud.com';
$port = 19585;                                       
$db   = 'defaultdb';
$user = 'avnadmin';
$pass = 'AVNS_LkZJFyZ-FHyzsX7h5_J';

$dsn = "mysql:host=$host;port=$port;dbname=$db;charset=utf8mb4";

$pdo = new PDO($dsn, $user, $pass, [
    PDO::MYSQL_ATTR_SSL_CA => 'C:\Users\felix\Documents\Plandeldeman\PlandelApp\Assets\Web\ca.pem',
    PDO::MYSQL_ATTR_SSL_VERIFY_SERVER_CERT => true,
    PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
]);
?>