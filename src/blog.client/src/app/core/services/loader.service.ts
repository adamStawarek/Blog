import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Injectable({
    providedIn: 'root'
})
export class LoaderService {
    public isLoading$: Observable<boolean>;

    private _activeRequestsCount = 0;
    private _isLoadingSubject = new BehaviorSubject<boolean>(false);

    constructor() {
        this.isLoading$ = this._isLoadingSubject.asObservable();
    }

    public showLoader(): void {
        if (this._activeRequestsCount === 0) {
            this._isLoadingSubject.next(true);
        }
        this._activeRequestsCount++;
    }

    public hideLoader(): void {
        this._activeRequestsCount--;
        if (this._activeRequestsCount === 0) {
            this._isLoadingSubject.next(false);
        }
    }
}