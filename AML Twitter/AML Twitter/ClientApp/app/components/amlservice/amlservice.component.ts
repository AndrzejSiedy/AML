import { Component, Inject } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Headers, RequestOptions  } from '@angular/http';

@Component({
    selector: 'amlservice',
    templateUrl: './amlservice.component.html'
})
export class AmlServiceComponent {

    baseUrl: string;
    http: Http;
    headers: Headers;
    options: RequestOptions;


    public serviceStarted: boolean = false;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {

        this.baseUrl = baseUrl;
        this.http = http;

        this.headers = new Headers({ 'Content-Type': 'application/json' });
        this.options = new RequestOptions({ headers: this.headers });
        
    }

    public startAmlService() {
        console.warn('call AML service to start');
        this._callService('api/AmlService/StartAmlService', "start");
    }

    public stopAmlService() {
        console.warn('call AML service to stop');
        this._callService('api/AmlService/StopAmlService', "stop");
    }

    _callService(url: string, method: string) {
        return this.http.post(this.baseUrl + url, {}, this.options).subscribe(result => {
            console.warn('called service', url);
            console.warn('service response', result);

            this.serviceStarted = method == "start";

        }, error => console.error(error));
    }

}

