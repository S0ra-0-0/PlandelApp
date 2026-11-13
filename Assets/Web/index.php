<html>
<header>
    <link rel="stylesheet" href="Style/main.css">
    
    <script src="JS/optionsControl.js" defer></script>
</header>

<body>
    <div id="sideBar">
        <img id="sideBarBanner" src="img/plandelbanner.jpg">
        <ul>
            <li><a>Speciale Afbeeldingen</a></li>
            <li><a>Lobbies</a></li>
            <li><a>Info</a></li>
            <li><a>augghh idk</a></li>
        </ul>
    </div>

    <main>
        <div id="imgHolderMain">
            <?php
            include_once("Database/getImages.php");
            ?>
        </div>
    </main>

    <div id="imageOptions">
        
        <div id="topOptions">
            <h1>Opties</h1>
            <button id="optionControl">
                <img src="img/icons/remove.png">
            </button>
        </div>

        <div id="imgInfo">
            <img>
            <button id="imgDownload">DOWNLOAD</button>
        </div>
    </div>

</body>

</html>