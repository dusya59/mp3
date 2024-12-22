using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace mp3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {          
            await webView.EnsureCoreWebView2Async(null);          
            string htmlContent = $@"
    <!DOCTYPE html>
    <html lang=""ru"">
    <head>
        <meta charset = ""UTF-8"">
        <link rel =""stylesheet"" href = ""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"">
        <title > Music player app</title>
        <style>
            @import url('https://fonts.googleapis.com/css2?family=Ubuntu:wght@300;400;500;700&display=swap');
html{{
    box-sizing: border-box;
}}

body{{
    margin:0;
    font-family: 'Ubuntu', sans-serif;
    font-size: 12px;
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
}}
.background{{
    position:fixed;
    height: 200%;
    width: 200%;
    top:-50%;
    left:-50%;
    z-index: -1;
}}
.background>img{{
    position: absolute;
    margin: auto;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    min-width: 50%;
    min-height: 50%;
    filter:blur(15px);
    -webkit-filter: blur(50px);
    transform:scale(1.1);
}}
.container{{
    background-color: #e7e7e7;
    height: 500px;
    width: 400px;
    border-radius: 20px;
    box-shadow: 0 15px 30px rgba(0,0,0,0.3);
    transition: all 0.5s ease;
}}
.container:hover {{
    box-shadow: 0 15px 30px rgba(0,0,0,0.6);
}}
.player-img {{
    width: 300px;
    height: 300px;
    position: relative;
    top: -50px;
    left: 50px;
    border-radius: 20px;
    transition: all 0.5s ease; 
}}
.player-img img {{
    object-fit: cover;
    border-radius: 20px;
    height: 0;
    width:0;
    opacity: 0;
    box-shadow: 0 5px 30px rgba(0,0,0,.5); 
    
}}
.player-img:hover{{
    box-shadow: 0 5px 30px rgba(0,0,0,.8);
}}
.player-img img.active{{
    width:100%;
    height:100%;
    transition: all .5s;
    opacity:1;
}}
#volumeRange {{
    width:300px;
    margin:10px 50px 0 50px;
}}
h2{{
    font-size: 25px;
    text-align: center;
    font-weight: 500;
    margin: 0 50px 0;
}}
h3{{
    font-size: 18px;
    text-align: center;
    font-weight: 500;
    margin: 10px 0 0;
}}

.player-progress {{
    background-color: #fff;
    border-radius: 5px;
    cursor: pointer;
    margin: 25px 20px 35px;
    height: 6px;
    width: 90%;
}}
.progress {{
    background-color: #212121;
    border-radius: 5px;
    height:100% ;
    width: 0%;
    transition: width 0.1 linear;
}}
.music-duration{{
    position: relative;
    top: -25px;
    display:flex;
    justify-content: space-between;
}}

.player-controls{{
    position: relative;
    top: -15px;
    left: 120px;
    width: 200px;
}}
.fa-solid{{
    font-size: 30px;
    color: #666;
    margin-right: 30px;
    cursor: pointer;
    user-select: none;
    transition: all .3s ease;
}}
.fa-solid:hover {{
    filter: brightness(40%);
}}
.play-button {{
    font-size: 44px;
    position: relative;
    top: 3px; 
    
}}
        </style>
    </head>
    <body>
        <div class=""background"">
            <img src="""" id=""bg-img"">
        </div>

        <div class=""container"">
            <div class=""player-img"">
                <img src="""" class=""active"" id=""cover"">
            </div>

            <h2 id=""music-title""></h2>
            <h3 id=""music-artist""></h3>
            <input type=""range"" min=""0"" max=""1"" step=""0.01"" value=""0.5"" id=""volumeRange"">
            <div class=""player-progress"" id=""player-progress"">
                <div class=""progress"" id=""progress""></div>
                <div class=""music-duration"">
                    <span id=""current-time"">0:00</span>
                    <span id=""duration"">0:00</span>
                </div>
            </div>

            <div class=""player-controls"">
                <i class=""fa-solid fa-backward"" id=""prev""></i>
                <i class=""fa-solid fa-play"" id=""play""></i>
                <i class=""fa-solid fa-forward"" id=""next""></i>
            </div>
        </div>
        <script>
const image = document.getElementById('cover'), 
title = document.getElementById('music-title'),
artist = document.getElementById('music-artist'),
currentTimeEl = document.getElementById('current-time'),
durationEl = document.getElementById('duration'),
progress = document.getElementById('progress'),
playerProgress = document.getElementById('player-progress'),
prevBtn = document.getElementById('prev'),
nextBtn = document.getElementById('next'),
playBtn = document.getElementById('play'),
background = document.getElementById('bg-img'),
volumeSlider = document.getElementById(""volumeSlider"");

const music = new Audio()

const songs = [
    {{  path:'https://mp3.local/untitled.mp3',
        displayName:'one by one',
        cover: 'https://mp3.local/untitled143.jpg',
        artist:'former hero',
    }},
    {{
        path:'https://mp3.local/carefree.mp3',
        displayName:'carefree',
        cover: 'https://mp3.local/carefree.jpg',
        artist:'Sqwore',
    }},
    {{
        path:'https://mp3.local/radiohead.mp3',
        displayName:'All I Need',
        cover: 'https://mp3.local/radiohead.jpg',
        artist:'Radiohead',
    }},
    {{path: 'https://mp3.local/1.mp3',
        displayName: 'Wish',
        cover: 'https://mp3.local/sign%20crush%20motorist.jpg',
        artist: 'sign crushes motorist',
    }},
    {{path: 'https://mp3.local/4.mp3',
        displayName: 'TakeItFromMe',
        cover: 'https://mp3.local/bones.jpg',
        artist: 'BONES',
    }}
];

let musicIndex = 0;
let isPLaying = false;

function TogglePlay(){{
    if(isPLaying){{
        pauseMusic();
    }}else{{
        playMusic();
    }}    
}}

function playMusic() {{
    isPLaying = true;
    playBtn.classList.remove('fa-play');
    playBtn.classList.add('fa-pause');
    music.play();
  }}
  
  function pauseMusic() {{
    isPLaying = false;
    playBtn.classList.remove('fa-pause');
    playBtn.classList.add('fa-play');
    music.pause();
  }}

function loadMusic() {{
    const song = songs[musicIndex]; 
    music.src = song.path;
    title.textContent = song.displayName;
    artist.textContent = song.artist;
    image.src = song.cover;
    background.src = song.cover; 
  }}

  function changeMusic(direction) {{
    musicIndex = (musicIndex + direction + songs.length) % songs.length;
    loadMusic(songs[musicIndex]);
    playMusic();
  }}

function updateProgressBar(){{
    const {{duration, currentTime}} = music;
    const progressPercent = (currentTime / duration) * 100;
    progress.style.width = `${{progressPercent}}%`;

    const formatTime = (time)=> String(Math.floor(time)).padStart(2,'0');
    durationEl.textContent = `${{formatTime(duration /60)}}: ${{formatTime(duration % 60)}}`;
    currentTimeEl.textContent = `${{formatTime(currentTime /60)}}: ${{formatTime(currentTime % 60)}}`;
}}

function setProgressBar(e){{
    const width = playerProgress.clientWidth;
    const clickX = e.offsetX;
    music .currentTime = (clickX / width) * music.duration;
}}


let volumeRange = document.getElementById(""volumeRange"");

volumeRange.addEventListener(""input"", function() {{
    music.volume = volumeRange.value;
}});

    

playBtn.addEventListener('click', TogglePlay);
prevBtn.addEventListener('click', ()=> changeMusic(-1));
nextBtn.addEventListener('click', ()=> changeMusic(1));
music.addEventListener('ended', () => changeMusic(1));
music.addEventListener('timeupdate', updateProgressBar);
playerProgress.addEventListener('click', setProgressBar);
loadMusic(songs[musicIndex]);
</script>
    </body>
    </html>";

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string FolderPath = Path.Combine(baseDirectory, "mp3");

            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "mp3.local",
                FolderPath,
                CoreWebView2HostResourceAccessKind.Allow
            );
            webView.NavigateToString(htmlContent);
        }
    } 
}
