import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Injectable({
    providedIn: 'root'
})
export class LoaderService {
    public isLoading$;

    private _apiCount = 0;
    private _isLoadingSubject = new BehaviorSubject<boolean>(false);

    constructor() {
        this.isLoading$ = this._isLoadingSubject.asObservable();
    }

    public showLoader(): void {
        if (this._apiCount === 0) {
            this._isLoadingSubject.next(true);
        }
        this._apiCount++;
    }

    public hideLoader(): void {
        this._apiCount--;
        if (this._apiCount === 0) {
            this._isLoadingSubject.next(false);
        }
    }
}