/* eslint-disable @typescript-eslint/no-explicit-any */
import { ErrorHandler, Injectable, NgZone } from "@angular/core";
import { MatSnackBar, } from '@angular/material/snack-bar';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(private zone: NgZone, private _snakBar: MatSnackBar) { }

    public handleError(error: any): void {
        const defaultError = 'An error occurred. Please try again later.';
        let msg = defaultError;
        if (error.response != null) {
            try {
                const errorResponse = JSON.parse(error.response);
                msg = errorResponse?.detail ?? msg;
            } catch {
                msg = defaultError;
            }
        }

        this.zone.run(() =>
            this._snakBar.open(msg, 'Close', {
                duration: 5000,
                panelClass: ['notification-error']
            })
        );
    }
}