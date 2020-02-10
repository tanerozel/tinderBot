import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})


export class HomeComponent {
  bots = BOTS;
  Baseurl: string;
  constructor( @Inject('BASE_URL') baseUrl: string) {
    this.Baseurl = baseUrl;
 
  }

  addBot() {
    this.bots.push(Object.assign({}, this.bots[0]));
  }

  
  terminateBot(bot) {
    bot.status = false;
  }

  runBot(bot) {
    fetch(this.Baseurl + "api/bot/run", {
      headers: {
        'X-Auth-Token': bot.token
      }
    }).then((response) => {
      response.json().then((result) => {

        if (result.ReasonPhrase) {
       
          alert(result.ReasonPhrase)
          bot.status = false;
        }

        bot.totaluser = bot.totaluser + result.data.results.length;
        if (bot.status) {
          var that = this;
          setTimeout(function () {
            that.runBot(bot);

          }, 5000)
         
        }
       
      });

    })
  } 

}

interface Bot {
  name: string;
  location: string;
  status: boolean;
  totaluser: number;
  token: string
}

const BOTS: Bot[] = [
  {
    name: 'İstanbul Esenler',
    location: 'f/f3/Flag_of_Russia.svg',
    status: false,
    totaluser: 0,
    token: "e4415083-e579-4f90-8a19-ef822b26c562"
  }

];
