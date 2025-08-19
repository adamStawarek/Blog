import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';

export interface CanComponentDeactivate {
    canDeactivate: () => boolean | Observable<boolean>;
}

@Injectable({
    providedIn: 'root',
})
export class UnsavedChangesGuard implements CanDeactivate<CanComponentDeactivate> {
    public canDeactivate(component: CanComponentDeactivate): boolean | Observable<boolean> {
        if (component.canDeactivate && !component.canDeactivate()) {
            return confirm('You have unsaved changes. Do you really want to leave?');
        }

        return true;
    }
}