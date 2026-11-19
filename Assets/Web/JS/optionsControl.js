document.getElementById("optionControl").addEventListener("click", ToggleOptions);
let active = false;
let currentDownloadLink;

function ToggleOptions(card){

    console.log(active, card);

    if(!active){
        document.getElementById("imageOptions").style.display = "unset";
    }else{
        document.getElementById("imageOptions").style.display = "none";
    }

    if(card.className == "imageHolder"){
        console.log("xiao hong shu");

        let image = card.querySelector("img").getAttribute("src");
        console.log(image);

        document.getElementById("imgInfo").querySelector("img").setAttribute("src", image);
        currentDownloadLink = image;
    }

    document.getElementById("imgDownload").addEventListener("click", () => {
        downloadImage(currentDownloadLink, currentDownloadLink);
    });

    document.getElementById("openBig").addEventListener("click", () => {
        window.open(currentDownloadLink);
    });

    active = !active;
}

function MakeButtonsWork() {
    document.querySelectorAll(".imageHolder").forEach(function (card) {
        console.log("added button thingie to: " + card);
        card.addEventListener("click", function () {
            ToggleOptions(card);
        });
    });
}

MakeButtonsWork();

for(i = 0; i < 2; i++){
    ToggleOptions();
}

async function downloadImage(url, filename){
    const res = await fetch(url);
    const blob = await res.blob();

    const a = document.createElement("a");
    a.href = URL.createObjectURL(blob);
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(a.href);
}

document.getElementById("imgDownload").addEventListener("click", )

