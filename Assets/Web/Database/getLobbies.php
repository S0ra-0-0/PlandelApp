<?php
include_once("connect.php");

$statement = $pdo->query("SELECT LobbyID, LobbyCode, Seed FROM lobbies.Lobby");
$objects = $statement->fetchAll(PDO::FETCH_ASSOC);

if(count($objects) > 0){
    foreach($objects as $row){
        echo 
        "<div class='lobbyInfoContainer' thisID='".$row["LobbyID"]."'>
            <h2>Lobby ".$row["LobbyID"]."</h2>
            <p><a title='copy lobby code'>Lobby code: ".$row["LobbyCode"]."</a></p>
            <p><a title='copt lobby seed'>Lobby seed: ".$row["Seed"]."</a></p>

            <div>
                <button>Stop lobby</button> 
            </div>
        </div>";
    }
}

?>