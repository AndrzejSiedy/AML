import { Component } from '@angular/core';

@Component({
    selector: 'counter',
    templateUrl: './counter.component.html'
})
export class CounterComponent {
    public currentCount = 0;

    constructor() {
        this.incrementCounterInterval();
    }

    public incrementCounterInterval() {
        setInterval(() => {
            this.incrementCounter();
        }, 5000);
    }

    public incrementCounter() {
        this.currentCount++;
    }
}
