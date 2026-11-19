<html>
<header>
    <link rel="stylesheet" href="Style/main.css">
    
    <script src="JS/optionsControl.js" defer></script>
</header>

<body>
    <?php
    include_once("templates/sideBar.php");
    ?>

    <main id="lobbyMain">
        <?php
        include_once("Database/getLobbies.php");
        ?>
    </main>

</body>

</html>