/* eslint-disable @typescript-eslint/no-explicit-any */
import { ErrorHandler, Injectable, NgZone } from "@angular/core";
import { MatSnackBar, } from '@angular/material/snack-bar';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(private zone: NgZone, private _snakBar: MatSnackBar) { }

    public handleError(error: any): void {
        let msg = error.message ?? 'An error occurred';
        if (error.response != null) {
            const errorResponse = JSON.parse(error.response);
            msg = errorResponse?.detail ?? msg;
        }

        this.zone.run(() =>
            this._snakBar.open(msg, 'Close', {
                duration: 5000,
                panelClass: ['error-snackbar', 'mat-warn']
            })
        );
    }
}