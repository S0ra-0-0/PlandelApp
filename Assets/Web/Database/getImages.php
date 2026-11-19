<?php
include_once("connect.php");

$statement = $pdo->query("SELECT ObjectID, ObjectName, ObjectImagePath, ObjectDescription FROM lobbies.SpecialeObjecten");
$objects = $statement->fetchAll(PDO::FETCH_ASSOC);

if(count($objects) > 0){
    foreach($objects as $row){
        echo "<div class='imageHolder'>";
            echo "<img src=".$row["ObjectImagePath"].">";
            echo "<h2>".$row["ObjectName"]."</h2>";
            echo "<p>".$row["ObjectDescription"]."</p>";
        echo "</div>";
    }
}
?>