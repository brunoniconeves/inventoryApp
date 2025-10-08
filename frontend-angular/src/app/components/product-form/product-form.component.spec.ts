import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';
import { ProductFormComponent } from './product-form.component';
import { ProductStore } from '../../store/product.store';

describe('ProductFormComponent', () => {
  let component: ProductFormComponent;
  let fixture: ComponentFixture<ProductFormComponent>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockSnackBar: jasmine.SpyObj<MatSnackBar>;
  let mockStore: jasmine.SpyObj<any>;

  beforeEach(async () => {
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockSnackBar = jasmine.createSpyObj('MatSnackBar', ['open']);
    mockStore = {
      products: jasmine.createSpy().and.returnValue([]),
      selectedProduct: jasmine.createSpy().and.returnValue(null),
      selectProduct: jasmine.createSpy(),
      createProduct: jasmine.createSpy(),
      updateProduct: jasmine.createSpy(),
    };

    await TestBed.configureTestingModule({
      imports: [
        ProductFormComponent,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: MatSnackBar, useValue: mockSnackBar },
        { provide: ProductStore, useValue: mockStore },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => 'new'
              }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with empty values in create mode', () => {
    expect(component.productForm.get('name')?.value).toBe('');
    expect(component.productForm.get('description')?.value).toBe('');
    expect(component.productForm.get('price')?.value).toBe(0);
    expect(component.isEditMode()).toBe(false);
  });

  it('should validate required fields', () => {
    const form = component.productForm;

    expect(form.valid).toBeFalse();

    form.get('name')?.setValue('');
    expect(form.get('name')?.hasError('required')).toBeTrue();

    form.get('description')?.setValue('');
    expect(form.get('description')?.hasError('required')).toBeTrue();

    form.get('price')?.setValue('');
    expect(form.get('price')?.hasError('required')).toBeTrue();
  });

  it('should validate minimum length for name', () => {
    const nameControl = component.productForm.get('name');

    nameControl?.setValue('AB');
    expect(nameControl?.hasError('minlength')).toBeTrue();

    nameControl?.setValue('ABC');
    expect(nameControl?.hasError('minlength')).toBeFalse();
  });

  it('should validate minimum length for description', () => {
    const descControl = component.productForm.get('description');

    descControl?.setValue('Short');
    expect(descControl?.hasError('minlength')).toBeTrue();

    descControl?.setValue('Long enough description');
    expect(descControl?.hasError('minlength')).toBeFalse();
  });

  it('should validate minimum price', () => {
    const priceControl = component.productForm.get('price');

    priceControl?.setValue(0);
    expect(priceControl?.hasError('min')).toBeTrue();

    priceControl?.setValue(0.01);
    expect(priceControl?.hasError('min')).toBeFalse();
  });

  it('should call createProduct when form is submitted in create mode', (done) => {
    component.productForm.setValue({
      name: 'Test Product',
      description: 'Test Description for product',
      price: 99.99
    });

    component.onSubmit();

    expect(mockStore.createProduct).toHaveBeenCalledWith({
      name: 'Test Product',
      description: 'Test Description for product',
      price: 99.99
    });

    setTimeout(() => {
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/products']);
      done();
    }, 600);
  });

  it('should navigate back on cancel', () => {
    component.onCancel();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/products']);
  });

  it('should mark all fields as touched when submitting invalid form', () => {
    component.productForm.setValue({
      name: '',
      description: '',
      price: 0
    });

    component.onSubmit();

    expect(component.productForm.get('name')?.touched).toBeTrue();
    expect(component.productForm.get('description')?.touched).toBeTrue();
    expect(component.productForm.get('price')?.touched).toBeTrue();
  });
});

