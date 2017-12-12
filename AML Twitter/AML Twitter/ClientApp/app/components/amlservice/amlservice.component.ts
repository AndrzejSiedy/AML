import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'amlservice',
    templateUrl: './amlservice.component.html'
})
export class AmlServiceComponent {
    public forecasts: WeatherForecast[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
            this.forecasts = result.json() as WeatherForecast[];
        }, error => console.error(error));
    }

    public startAmlService() {
        console.warn('call AML service to start');
    }
    public stopAmlService() {
        console.warn('call AML service to stop');
    }

}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
