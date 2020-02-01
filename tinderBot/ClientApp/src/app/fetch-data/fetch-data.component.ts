import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];
  public Baseurl: string;
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.Baseurl = baseUrl;
    this.req();
  }
  req() {
    fetch(this.Baseurl + "api/bot/run", {
      headers: {
        'X-Auth-Token': "e4415083-e579-4f90-8a19-ef822b26c562"
      }}).then((response) => {
      response.json().then((result) => {
        this.req();
      });

    })
  }

}
 

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
