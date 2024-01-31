import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountServicve = inject(AccountService);
  const toast= inject(ToastrService)

  return accountServicve.currentUser$.pipe(
    map(user => {
      if(user) return true;
      else{
        toast.error('you shall not pass');
        return false;
      }
    })
  )
};

