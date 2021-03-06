/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
 * Changes may cause incorrect behavior and will be lost if the code is
 * regenerated.
 */

'use strict';

/**
 * A X509 certificate revocation list.
 *
 */
class X509CrlApiModel {
  /**
   * Create a X509CrlApiModel.
   * @property {string} [issuer] The Issuer name of the revocation list.
   * @property {object} crl The certificate revocation list.
   */
  constructor() {
  }

  /**
   * Defines the metadata of X509CrlApiModel
   *
   * @returns {object} metadata of X509CrlApiModel
   *
   */
  mapper() {
    return {
      required: false,
      serializedName: 'X509CrlApiModel',
      type: {
        name: 'Composite',
        className: 'X509CrlApiModel',
        modelProperties: {
          issuer: {
            required: false,
            serializedName: 'issuer',
            type: {
              name: 'String'
            }
          },
          crl: {
            required: true,
            serializedName: 'crl',
            type: {
              name: 'Object'
            }
          }
        }
      }
    };
  }
}

module.exports = X509CrlApiModel;
