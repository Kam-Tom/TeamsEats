import React, { useState, useContext, ChangeEvent } from 'react';
import {
    Dialog,
    DialogTrigger,
    DialogSurface,
    DialogTitle,
    DialogBody,
    DialogActions,
    DialogContent,
    Button,
    Field,
    Input,
    makeStyles
} from "@fluentui/react-components";
import { TeamsFxContext } from '../Context';

interface FormData {
    dish: string;
    price: string;
    additionalInfo: string;
    orderId: number;
}

interface FormErrors {
    dish?: string;
    price?: string;
}

interface ItemFormProps {
    orderId: number;
}

const useStyles = makeStyles({
    formSurface: {
        width: '400px',
    },
    formContent: {
        display: 'flex',
        flexDirection: 'column',
        gap: '10px',
    },
    field: {
        marginBottom: '10px',
    },
    formButtons: {
        display: 'flex',
        justifyContent: 'flex-end',
        gap: '10px',
    },
});

const ItemForm: React.FC<ItemFormProps> = ({ orderId }) => {
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const [formData, setFormData] = useState<FormData>({
        dish: '',
        price: '',
        additionalInfo: '',
        orderId: orderId
    });
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [errors, setErrors] = useState<FormErrors>({});
    const classes = useStyles();

    const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        setFormData({ ...formData, [name]: value });
    };

    const validateForm = (): boolean => {
        const newErrors: FormErrors = {};
        if (!formData.dish) {
            newErrors.dish = 'Dish name is required.';
        }
        if (!formData.price || parseFloat(formData.price) <= 0) {
            newErrors.price = 'Price must be a positive number.';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async () => {
        if (!validateForm()) return;

        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            const formattedData = {
                dish: formData.dish,
                price: parseFloat(formData.price),
                additionalInfo: formData.additionalInfo,
                orderId: orderId
            };
            const response = await fetch('https://localhost:7125/item', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token!.token}`
                },
                body: JSON.stringify(formattedData)
            });

            if (response.ok) {
                setIsDialogOpen(false);
            } else {
                console.error(`Failed to submit form: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during form submission:', error);
        }
    };

    return (
        <Dialog open={isDialogOpen} onOpenChange={(_, data) => setIsDialogOpen(data.open)}>
            <DialogTrigger disableButtonEnhancement>
                <Button onClick={() => setIsDialogOpen(true)}>Add Item</Button>
            </DialogTrigger>
            <DialogSurface className={classes.formSurface}>
                <DialogBody>
                    <DialogTitle>Add Order Item</DialogTitle>
                    <DialogContent className={classes.formContent}>
                        <Field className={classes.field}
                            label="Dish Name"
                            validationState={errors.dish ? 'error' : 'none'}
                            validationMessage={errors.dish}
                        >
                            <Input
                                id="dish"
                                name="dish"
                                value={formData.dish}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Price"
                            validationState={errors.price ? 'error' : 'none'}
                            validationMessage={errors.price}
                        >
                            <Input
                                id="price"
                                name="price"
                                type="number"
                                value={formData.price}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Additional Info"
                        >
                            <Input
                                id="additionalInfo"
                                name="additionalInfo"
                                value={formData.additionalInfo}
                                onChange={handleChange}
                            />
                        </Field>
                    </DialogContent>
                    <DialogActions className={classes.formButtons}>
                        <DialogTrigger disableButtonEnhancement>
                            <Button appearance="secondary">Close</Button>
                        </DialogTrigger>
                        <Button appearance="primary" onClick={handleSubmit}>Submit</Button>
                    </DialogActions>
                </DialogBody>
            </DialogSurface>
        </Dialog>
    );
}

export default ItemForm;
