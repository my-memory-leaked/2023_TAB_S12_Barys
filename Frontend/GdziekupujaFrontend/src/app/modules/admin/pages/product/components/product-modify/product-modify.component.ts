import { Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { Category, Product } from '@modules/offers/interfaces/offers.interface';
import { AbstractControl, ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { AdminStorageService } from '@modules/admin/services/admin-storage.service';
import { ProductFormHandlerService } from '@modules/admin/pages/product/services/product-form-handler.service';
import { AdminSubmitFormService } from '@modules/admin/services/admin-submit-form.service';

@Component({
  selector: 'product-modify',
  templateUrl: './product-modify.component.html',
  styleUrls: ['./product-modify.component.scss'],
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: FormGroupDirective,
    },
  ],
})

export class ProductModifyComponent implements OnInit {

  form: FormGroup;
  products: Observable<Product[]>;
  additionalProperties: AdditionalProperties[] = [];

  constructor(
    private controlContainer: ControlContainer,
    private productFormHandlerService: ProductFormHandlerService,
    private adminStorageService: AdminStorageService,
    private adminSubmitFormService: AdminSubmitFormService,
  ) { }

  ngOnInit(): void {
    this.form = this.controlContainer.control as FormGroup;
    this.productFormHandlerService.setFormGroupForProductModify(this.form);

    this.products = this.adminStorageService.products$.asObservable();

    this.adminSubmitFormService.getClearData().subscribe(() => {
      this.form.reset();
      this.additionalProperties = [];
    });
  }

  get propertyName(): AbstractControl {
    return this.form.get('propertyName');
  }

  get property(): AbstractControl {
    return this.form.get('property');
  }

  get availableProps(): AbstractControl {
    return this.form.get('availableProps');
  }

  mergeAndAddAdditionalInfo(): void {
    if (this.propertyName.value && this.property.value) {

      if (this.additionalProperties.findIndex((res) => res.propertyName === this.propertyName.value) === -1) {
        //nie ma jeszcze takiej property
        this.additionalProperties.unshift({ propertyName: this.propertyName.value, properties: [this.property.value] });
      } else {

        this.additionalProperties = this.additionalProperties.map((res) => {
          if (res.propertyName === this.propertyName.value && res.properties.findIndex((result) => result === this.property.value) === -1) {
            return {
              ...res,
              properties: [this.property.value, ...res.properties],
            }
          }

          return res;
        })
      }

      this.additionalProperties = this.additionalProperties.map((res) => {
        let propertiesFixed = '';
        res.properties.forEach((property, index) => {
          propertiesFixed += '"' + property + '", ';
          if (index === res.properties.length - 1) {
            propertiesFixed = propertiesFixed.slice(0, -2);
          }
        });

        return {
          ...res,
          displayProperty: '"' + res.propertyName + '": ' + propertiesFixed,
        }
      })

      this.setFormValue(this.additionalProperties);

      this.propertyName.reset();
      this.property.reset();
    }
  }

  removeAdditionalInfo(info: AdditionalProperties): void {
    this.additionalProperties = this.additionalProperties.filter((res) => res !== info);
    this.setFormValue(this.additionalProperties);
  }

  private setFormValue(properties: AdditionalProperties[]): void {
    const availableProps: { [key: string]: string[] } = {};

    properties.forEach((item: AdditionalProperties) => {
      availableProps[item.propertyName] = item.properties;
    });

    this.availableProps.setValue(availableProps);
  }
}

interface AdditionalProperties {
  propertyName: string,
  properties: string[];
  displayProperty?: string;
}